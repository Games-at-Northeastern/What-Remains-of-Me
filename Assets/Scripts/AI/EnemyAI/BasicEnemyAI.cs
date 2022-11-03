using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents and supports the behavior of enemy AI. 
/// </summary>
public class BasicEnemyAI : MonoBehaviour, IBehaviorTree
{
    public GameObject player;
    public EnemyBattery battery;
    public float followRadius;
    public float attackRadius;
    public float walkSpeed;
    public float pauseTime;
    public float attackTime;

    [SerializeField] private LayerMask platformLayerMask;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private Animator anim;
    private Node topNode;
    private bool isTurning;
    private bool isAlerted = false;
    private bool isFollowing = false;
    private bool isDead = false;
    private bool isAttacking = false;

    /// <summary>
    /// Initializes an AI with their behvaior, body,
    /// collision box, animation, and their batterys state.
    /// </summary>
    public void Start()
    {
        InitializeBehaviorTree();
        rb = this.GetComponent<Rigidbody2D>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        anim = this.GetComponent<Animator>();
        if (battery == null) battery = this.gameObject.GetComponent<EnemyBattery>();
    }


    /// <summary>
    /// Update the AI's movement every frame.
    /// </summary>
    public void Update()
    {
        Run();
        Move(Vector3.zero);
    }

    /// <summary>
    /// Create the behavior of an AI with their movement and pathing.
    /// </summary>
    public void InitializeBehaviorTree()
    {
        Node moveForward = new MoveForward(this);
        Node followPlayer = new FollowPlayer(this);
        Node playerInView = new PlayerInView(player.transform, this.gameObject.transform, followRadius);
        Node dead = new HandleDead(this, battery);

        List<Node> followNodes = new List<Node>();
        followNodes.Add(playerInView);
        followNodes.Add(followPlayer);
        Node followSequence = new Sequence(followNodes);

        List<Node> movementOptionNodes = new List<Node>();
        movementOptionNodes.Add(followSequence);
        movementOptionNodes.Add(moveForward);
        Node movementRoot = new Selector(movementOptionNodes);

        List<Node> deadOrMove = new List<Node>();
        deadOrMove.Add(dead);
        deadOrMove.Add(movementRoot);
        Node rootNode = new Selector(deadOrMove);

        topNode = rootNode;
    }

    /// <summary>
    /// Move the AI.
    /// </summary>
    public void Run()
    {
        topNode.Process();
    }

    /// <summary>
    /// Move the AI based on if its dead, close enough to attack,
    /// or currently attacking. If not attack set movement to base state.
    /// </summary>
    /// <param name="target">The target to move</param> 
    public void Move(Vector3 target)
    {
        if (isDead)
        {
            HandleEnemyDead();
        }
        else if (Vector3.Distance(player.transform.position, this.transform.position) <= attackRadius)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            StartCoroutine(Attack());
        }
        else if (!isAttacking)
        {
            if ((isFollowing || isAlerted))
            {
                StartCoroutine(MoveWithTarget());
            }
            else
            {
                MoveWithNoTarget();
            }

            UpdateAnimationState();
        }
    }

    /// <summary>
    /// Set the AI to an alerted state.
    /// </summary>
    public void Alerted()
    {
        isAlerted = true;
    }

    /// <summary>
    /// Helps the AI move when there is no target present.
    /// </summary>
    private void MoveWithNoTarget()
    {
        if (IsGrounded() || isTurning)
        {
            rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
            if (IsGrounded())
            {
                isTurning = false;
            }
        }
        else
        {
            isTurning = true;
            walkSpeed *= -1;
        }
    }

    /// <summary>
    /// Helps the AI move in relation to its target.
    /// </summary>
    /// <returns>the pause time for the AI</returns>
    private IEnumerator MoveWithTarget()
    {
        if (walkSpeed < 0) walkSpeed *= -1;
        float currentVel = this.transform.position.x > player.transform.position.x ? -walkSpeed : walkSpeed;
        if (currentVel != rb.velocity.x)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            yield return new WaitForSeconds(pauseTime);
        }

        if (IsGrounded())
        {
            rb.velocity = new Vector2(currentVel, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    /// <summary>
    /// Handles the Attacking state of the Ai. 
    /// </summary>
    /// <returns>The wait period after the AI performs the attack</returns>
    private IEnumerator Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetBool("isAttacking", true);
            this.gameObject.layer = 9;
            yield return new WaitForSeconds(attackTime);
            anim.SetBool("isAttacking", false);
            this.gameObject.layer = 13;
            isAttacking = false;
        }
    }

    /// <summary>
    /// Change the AI's velocity when they are dead.
    /// </summary>
    private void HandleEnemyDead()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }


    /// <summary>
    /// Update the Animation of the AI based on velocity and if its dead.
    /// </summary>
    private void UpdateAnimationState()
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        if (rb.velocity.x < 0) { sr.flipX = true; }
        if (rb.velocity.x > 0) { sr.flipX = false; }
        if (isDead) { }
    }

    /// <summary>
    /// Changes the color on the right or left
    /// depending on the side's collision state
    /// </summary>
    /// <returns>true if the left and right colliders are both not null</returns>
    private bool IsGrounded()
    {
        float extraHeight = 0.5f;
        Vector3 raycastOffset = new Vector3(-0.3f, 0, 0);
        RaycastHit2D raycastHitLeft = Physics2D.Raycast(boxCollider2D.bounds.center - raycastOffset, Vector2.down, boxCollider2D.bounds.extents.y + extraHeight, platformLayerMask);
        RaycastHit2D raycastHitRight = Physics2D.Raycast(boxCollider2D.bounds.center + raycastOffset, Vector2.down, boxCollider2D.bounds.extents.y + extraHeight, platformLayerMask);
        Color rayColorLeft;
        Color rayColorRight;
        if (raycastHitLeft.collider != null)
        {
            rayColorLeft = Color.green;
        }
        else
        {
            rayColorLeft = Color.red;
        }

        if (raycastHitRight.collider != null)
        {
            rayColorRight = Color.green;
        }
        else
        {
            rayColorRight = Color.red;
        }
        Debug.DrawRay(boxCollider2D.bounds.center - raycastOffset, Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColorLeft);
        Debug.DrawRay(boxCollider2D.bounds.center + raycastOffset, Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColorRight);
        return raycastHitLeft.collider != null && raycastHitRight.collider != null;
    }

    /// <summary>
    /// Class that represents if the player can be seen by the AI
    /// </summary>
    public class PlayerInView : Node
    {
        private Transform player;
        private Transform enemy;
        private float radius;

        /// <summary>
        /// Constructs a PlayerInView object to check if the AI can see the player
        /// </summary>
        /// <param name="p">Represents the player character</param>
        /// <param name="e">Represents the enemy AI</param>
        /// <param name="r">Represents the radius between the player and enemy</param>
        public PlayerInView(Transform p, Transform e, float r)
        {
            player = p;
            enemy = e;
            radius = r;
        }

        /// <summary>
        /// Checks if enemy can see the player
        /// </summary>
        /// <returns>If the enemy player is in view</returns>
        public override bool Process()
        {
            if (Vector3.Distance(player.position, enemy.position) <= radius)
            {
                _isSuccess = true;
                return true;
            }
            _isSuccess = false;
            return false;
        }
    }



    /// <summary>
    /// Class that represents moving the AI forward.
    /// </summary>
    public class MoveForward : Node
    {
        BasicEnemyAI aiRef;

        /// <summary>
        /// Constructs a MoveForward object for the AI to move forward.
        /// </summary>
        /// <param name="aiRef">the Ai behavior reference</param>
        public MoveForward(BasicEnemyAI aiRef)
        {
            this.aiRef = aiRef;
        }


        /// <summary>
        /// Turns off the AI's following state.
        /// </summary>
        /// <returns>True</returns>
        public override bool Process()
        {
            aiRef.isFollowing = false;
            _isSuccess = true;
            return true;
        }
    }

    /// <summary>
    /// Class that represents the AI following the player character.
    /// </summary>
    public class FollowPlayer : Node
    {
        BasicEnemyAI aiRef;


        /// <summary>
        /// Constructs a FollowPlayer object to set the AI to follow the player.
        /// </summary>
        /// <param name="aiRef">The AI reference behavior</param>
        public FollowPlayer(BasicEnemyAI aiRef)
        {
            this.aiRef = aiRef;
        }


        /// <summary>
        /// Sets the AI to a player following state.
        /// </summary>
        /// <returns>True</returns>
        public override bool Process()
        {
            aiRef.isFollowing = true;
            _isSuccess = true;
            return true;
        }
    }

    /// <summary>
    /// Class that represents the dead state of the AI.
    /// </summary>
    public class HandleDead : Node
    {
        BasicEnemyAI aiRef;
        EnemyBattery battery;

        /// <summary>
        /// Constructs a HandleDead object to modify the behavior of dead AI
        /// </summary>
        /// <param name="aiRef">AI reference behavior to modify</param>
        /// <param name="battery">Battery state of the AI to check</param>
        public HandleDead(BasicEnemyAI aiRef, EnemyBattery battery)
        {
            this.aiRef = aiRef;
            this.battery = battery;
        }

        /// <summary>
        /// Sets isSuccess to if the battery is empty or at 1 (AI dead).
        /// </summary>
        /// <returns>the isSuccess result</returns>
        public override bool Process()
        {
            _isSuccess = (battery.GetPercentFull() == 0 || battery.GetPercentFull() == 1);
            aiRef.isDead = _isSuccess;
            return _isSuccess;
        }
    }
}
