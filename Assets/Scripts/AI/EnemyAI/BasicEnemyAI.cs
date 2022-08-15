using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTreeAI;

public class BasicEnemyAI : MonoBehaviour, BehaviorTree
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

    public void Start()
    {
        InitializeBehaviorTree();
        rb = this.GetComponent<Rigidbody2D>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        anim = this.GetComponent<Animator>();
        if (battery == null) battery = this.gameObject.GetComponent<EnemyBattery>();
    }

    public void Update()
    {
        Run();
        Move(Vector3.zero);
    }

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

    public void Run()
    {
        topNode.Process();
    }

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

    public void Alerted()
    {
        isAlerted = true;
    }

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

    private void HandleEnemyDead()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void UpdateAnimationState()
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        if (rb.velocity.x < 0) { sr.flipX = true; }
        if (rb.velocity.x > 0) { sr.flipX = false; }
        if (isDead) { }
    }

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

    public class PlayerInView : Node
    {
        private Transform player;
        private Transform enemy;
        private float radius;

        public PlayerInView(Transform p, Transform e, float r)
        {
            player = p;
            enemy = e;
            radius = r;
        }

        public override bool Process()
        {
            if (Vector3.Distance(player.position, enemy.position) <= radius)
            {
                isSuccess = true;
                return true;
            }
            isSuccess = false;
            return false;
        }
    }

    public class MoveForward : Node
    {
        BasicEnemyAI aiRef;

        public MoveForward(BasicEnemyAI aiRef)
        {
            this.aiRef = aiRef;
        }

        public override bool Process()
        {
            aiRef.isFollowing = false;
            isSuccess = true;
            return true;
        }
    }

    public class FollowPlayer : Node
    {
        BasicEnemyAI aiRef;

        public FollowPlayer(BasicEnemyAI aiRef)
        {
            this.aiRef = aiRef;
        }

        public override bool Process()
        {
            aiRef.isFollowing = true;
            isSuccess = true;
            return true;
        }
    }

    public class HandleDead : Node
    {
        BasicEnemyAI aiRef;
        EnemyBattery battery;

        public HandleDead(BasicEnemyAI aiRef, EnemyBattery battery)
        {
            this.aiRef = aiRef;
            this.battery = battery;
        }

        public override bool Process()
        {
            isSuccess = (battery.GetPercentFull() == 0 || battery.GetPercentFull() == 1);
            aiRef.isDead = isSuccess;
            return isSuccess;
        }
    }
}
