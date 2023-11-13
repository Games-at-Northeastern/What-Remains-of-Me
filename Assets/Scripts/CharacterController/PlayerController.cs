using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;
using More2DGizmos;
namespace PlayerControllerRefresh
{
    public class PlayerController : MonoBehaviour, ICharacterController


    {


        #region Internal References

        public PlayerSettings settings;
        public WireThrower wire;
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
        private Vector2 _speed;


        #endregion

        #region External

        public Vector2 Velocity => rb.velocity;

        public Vector3 position => this.transform.position;

        public bool Grounded => this.TouchingGround();

        public Vector2 Direction => this.transform.forward;
        public AnimationType GetAnimationState() => AnimationType.IDLE;

        public Facing LeftOrRight => direction();

        public Vector2 Speed => _speed;

        Vector2 ICharacterController.Speed { get => _speed; set => _speed = value; }

        private Facing direction()
        {
            if (playerInputs.MoveInput.x < 0)
            {
                return Facing.left;
            }
            return Facing.right;
        }

        private bool inputsLocked = false;
        public void LockInputs() => inputsLocked = true;
        public void UnlockInputs() => inputsLocked = false;





        #endregion

        #region StartUp
        private void OnValidate()
        {
            Start();
            Debug.Log("Validate");
        }
        [ContextMenu("Update Moves to New Settings")]
        private void SetupMoves()
        {
            groundMovement = new GroundMovement(settings.maxRunSpeed, settings.groundedAcceleration, settings.groundedDeceleration, this);
            airMovement = new AirMovement(settings.maxAirSpeed, settings.terminalVelocity, settings.airAcceleration, settings.fallGravity, this);
            jump = new Jump(settings.risingGravity, settings.jumpHeight, JumpType.setSpeed, this);
            dash = new Dash(this, settings.dashSpeedX, settings.dashTime);
            swing = new Swing(wire, this, settings.fallGravity, settings.wireSwingNaturalAccelMultiplier, settings.wireSwingMaxAngularVelocity, settings.wireSwingDecayMultiplier, settings.wireSwingDecayMultiplier, settings.wireSwingReferenceWireLength, settings.wireSwingManualAccelMultiplier, settings.wireLength);
            wallJump = new WallJump(this, settings.risingGravity, settings.WallJumpDistance, settings.takeControlAwayTime);
            wallSlide = new WallSlide(settings.wallSlideGravity, settings.maxWallSlideSpeed, this);
        }
        void Start()
        {
            col = GetComponent<CapsuleCollider2D>();
            SetupMoves();
            playerInputs = GetComponent<PlayerInputHandler>();
            rb = GetComponent<Rigidbody2D>();
        }

        #endregion

        public enum PlayerState
        {
            Grounded,
            Aerial,
            Swinging,
            OnWall,
        }
        private PlayerState currentState = PlayerState.Aerial;
        
        void FixedUpdate()
        {
            SwitchState();
            switch (currentState)
            {
                case PlayerState.Grounded:
                    groundMovement.UpdateDirection(playerInputs.MoveInput.x);
                    groundMovement.ContinueMove();
                    break;
                case PlayerState.Aerial:
                    airMovement.UpdateDirection(playerInputs.MoveInput);
                    airMovement.ContinueMove();
                    break;
                case PlayerState.OnWall:
                    wallSlide.ContinueMove();
                    break;
                case PlayerState.Swinging:
                    break;
            }
            Debug.Log("Jump?" + playerInputs.JumpPressed);
            if (!jump.IsMoveComplete())
            {
                jump.ContinueMove();
            }
            else
            {
                jump.CancelMove();
            }
            if (TouchingGround())
            {
                groundMovement.UpdateDirection(playerInputs.MoveInput.x);
                groundMovement.ContinueMove();
                if (playerInputs.JumpPressed)
                {
                    this._speed.y = 0;
                    jump.StartMove();
                }
            }
            else
            {
                airMovement.UpdateDirection(playerInputs.MoveInput);
                airMovement.ContinueMove();
            }


            Debug.Log("PCSPEED" + _speed);
            rb.velocity = Speed;
        }
        private void SwitchState()
        {
            if(GetNewState() == currentState)
            {
                return;
            }
            else
            {
                currentState = GetNewState();
            }
            switch (currentState)
            {
                case PlayerState.Grounded:
                    groundMovement.StartMove();
                    break;
                case PlayerState.Aerial:
                    airMovement.StartMove();
                    break;
                case PlayerState.OnWall:
                    wallSlide.StartMove();
                    break;
                case PlayerState.Swinging:
                    swing.StartMove();
                    break;
                default:
                    break;
            }
        }
        PlayerState GetNewState()
        {
            if (Grounded)
            {
                return PlayerState.Grounded;
            }
            else
            {
                if (wire.IsConnected())
                {
                    return PlayerState.Swinging;

                }
                else
                {
                    return PlayerState.Aerial;
                }
            }

            
        }


        #region Collision

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
