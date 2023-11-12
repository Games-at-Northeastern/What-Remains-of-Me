using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;
using More2DGizmos;
namespace PlayerControllerRefresh
{
    public class PlayerController : MonoBehaviour, ICharacterController


    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            Start();
        }
#endif


        #region External
        private Vector2 _speed;

        public Vector2 Velocity => rb.velocity;

        public Vector3 position => this.transform.position;

        public bool Grounded => this.TouchingGround();

        public Vector2 Direction => this.transform.forward;
        public AnimationType GetAnimationState() => AnimationType.IDLE;
        public Vector2 SetSpeed(Vector2 newSpeed) => _speed = newSpeed;

        Facing ICharacterController.LeftOrRight => LeftOrRight();

        public Vector2 Speed => _speed;

        private Facing LeftOrRight()
        {
            if(playerInputs.MoveInput.x < 0)
            {
                return Facing.left;
            }
            return Facing.right;
        }

        private bool inputsLocked = false;
        public void LockInputs() => inputsLocked = true;
        public void UnlockInputs() => inputsLocked = false;



        #region Collision
        public bool TouchingLeftWall()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);

            return GizmosPlus.BoxCast(origin + new Vector2(-leftOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.left, sideBounds.x - 0.01f, ~playerLayer);

        }
        public bool TouchingRightWall()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            return GizmosPlus.BoxCast(origin + new Vector2(rightOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.right, sideBounds.x - 0.01f, ~playerLayer);
        }
        public bool TouchingCeiling()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            return GizmosPlus.BoxCast(origin + new Vector2(0, ceilingOffset), new Vector2(ceilingBounds.x, 0.01f), 0, Vector2.up, ceilingBounds.y - 0.01f, ~playerLayer);
        }
        public bool TouchingGround()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            return GizmosPlus.BoxCast(origin + new Vector2(0, -groundOffset), new Vector2(groundBounds.x, 0.01f), 0, Vector2.down, groundBounds.y - 0.01f, ~playerLayer);

        }
        #endregion

        #endregion

        public PlayerSettings settings;
        [HideInInspector] Rigidbody2D rb;
        [HideInInspector] public CapsuleCollider2D col;
        public LayerMask playerLayer;
        GroundMovement groundMovement;
        AirMovement airMovement;
        Jump jump;
        Dash dash;
        Swing swing;
        Knockback knockback;
        WallJump wallJump;
        WallSlide wallSlide;

        PlayerInputHandler playerInputs;


        // Start is called before the first frame update
        void Start()
        {
            col = GetComponent<CapsuleCollider2D>();
            groundMovement = new GroundMovement(settings.maxRunSpeed, settings.groundedAcceleration, settings.groundedDeceleration, this);
            playerInputs = GetComponent<PlayerInputHandler>();
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            _speed = rb.velocity;
            groundMovement.UpdateDirection(playerInputs.MoveInput.x);
            groundMovement.ContinueMove();
            Debug.Log("PCSPEED" + _speed);
            rb.velocity = Speed;
        }
        [Header("CollisionDetectors")]
        [Tooltip("X is the width Y is the height")]
        [SerializeField] private Vector2 ceilingBounds;
        [Min(0)]
        [SerializeField] private float ceilingOffset;

        [Tooltip("X is the width Y is the height")]
        [SerializeField] Vector2 sideBounds;
        [Min(0)]
        [SerializeField] private float leftOffset;
        [SerializeField] private float rightOffset;

        [Tooltip("X is the width Y is the height")]
        [SerializeField] private Vector2 groundBounds;
        [Min(0)]
        [SerializeField] private float groundOffset;


        private void OnDrawGizmos()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            GizmosPlus.BoxCast(origin + new Vector2(rightOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.right, sideBounds.x - 0.01f, ~playerLayer);
            GizmosPlus.BoxCast(origin + new Vector2(-leftOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.left, sideBounds.x - 0.01f, ~playerLayer);
            GizmosPlus.BoxCast(origin + new Vector2(0, ceilingOffset), new Vector2(ceilingBounds.x, 0.01f), 0, Vector2.up, ceilingBounds.y - 0.01f, ~playerLayer);
            GizmosPlus.BoxCast(origin + new Vector2(0, -groundOffset), new Vector2(groundBounds.x, 0.01f), 0, Vector2.down, groundBounds.y - 0.01f, ~playerLayer);
        }


    }
}
