using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;
using More2DGizmos;
using System;

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
        [HideInInspector] public CheckpointManager checkpointManager;
        GroundMovement groundMovement;
        AirMovement airMovement;
        AirMovement airMovementZeroG;
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
        public AnimationType GetAnimationState()
        {
            switch (currentState)
            {
                case PlayerState.Grounded:
                    if (Mathf.Abs(playerInputs.MoveInput.x) > 0.1f)
                    {
                        return AnimationType.RUN;
                    }
                    else
                    {
                        return AnimationType.IDLE;
                    }
                    break;
                case PlayerState.Aerial:
                    if (_speed.y > 0)
                    {
                        return AnimationType.JUMP_RISING;
                    }
                    else
                    {
                        return AnimationType.JUMP_FALLING;
                    }
                    break;
                case PlayerState.OnWall:
                    break;
                case PlayerState.Swinging:
                    return AnimationType.WIRE_SWING;
                    break;
            }
            //Temporary
            return AnimationType.IDLE;
        }




        public Vector2 Speed => _speed;

        Vector2 ICharacterController.Speed { get => _speed; set => _speed = value; }

        public Facing LeftOrRight => direction();

        private Facing currentDirection = Facing.right;
        private Facing direction()
        {
            if (playerInputs.MoveInput.x == 0)
            {
                return currentDirection;
            }
            if (playerInputs.MoveInput.x < 0)
            {
                currentDirection = Facing.left;
                return Facing.left;
            }
            currentDirection = Facing.right;
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
        }
        [ContextMenu("Update Moves to New Settings")]
        private void SetupMoves()
        {
            groundMovement = new GroundMovement(settings.maxRunSpeed, settings.groundedAcceleration, settings.groundedDeceleration, this);
            airMovement = new AirMovement(settings.maxRunSpeed,settings.maxAirSpeed, settings.terminalVelocity, settings.airAcceleration,settings.airDeceleration , settings.fallGravity, this);
            airMovementZeroG = new AirMovement(settings.maxRunSpeed,settings.maxAirSpeed, settings.terminalVelocity, settings.airAcceleration, settings.airDeceleration,0, this);
            jump = new Jump(settings.risingGravity, settings.jumpHeight, JumpType.setSpeed, this);
            dash = new Dash(this, settings.dashSpeedX, settings.dashTime);
            swing = new Swing(wire, this, settings.fallGravity, settings.wireSwingNaturalAccelMultiplier, settings.SwingMaxAngularVelocity, settings.wireSwingDecayMultiplier, settings.wireSwingBounceDecayMultiplier, settings.PlayerSwayAccel, settings.wireLength);
            wallJump = new WallJump(this, settings.risingGravity, settings.WallJumpDistance, settings.takeControlAwayTime);
            wallSlide = new WallSlide(settings.wallSlideGravity, settings.maxWallSlideSpeed, this);
        }
        void Start()
        {
            col = GetComponent<CapsuleCollider2D>();
            SetupMoves();
            playerInputs = GetComponent<PlayerInputHandler>();
            rb = GetComponent<Rigidbody2D>();
            checkpointManager = FindObjectOfType<CheckpointManager>();

            FindObjectOfType<LevelManager>().OnPlayerReset.AddListener(Respawn);
            FindObjectOfType<LevelManager>().OnPlayerDeath.AddListener(Respawn);
            filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(~playerLayer);

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
            _speed = rb.velocity;
            SwitchState();
            switch (currentState)
            {
                case PlayerState.Grounded:
                    groundMovement.UpdateDirection(playerInputs.MoveInput.x);
                    groundMovement.ContinueMove();
                    HandleJump();
                    break;
                case PlayerState.Aerial:
                    if (jumped)
                    {
                        HandleJump();
                        airMovementZeroG.UpdateDirection(playerInputs.MoveInput);
                        airMovementZeroG.ContinueMove();
                    }
                    else
                    {
                        airMovement.UpdateDirection(playerInputs.MoveInput);
                        airMovement.ContinueMove();
                    }


                    break;
                case PlayerState.OnWall:
                    wallSlide.ContinueMove();
                    break;
                case PlayerState.Swinging:
                    swing.UpdateInput(playerInputs.MoveInput.x);
                    swing.ContinueMove();
                    if (playerInputs.JumpPressed)
                    {
                        wire.DisconnectWire();
                        Debug.Log("DC wire");
                    }
                    break;
            }


            //Debug.Log("PCSPEED" + _speed);
            rb.velocity = Speed;
        }
        private void SwitchState()
        {
            if (GetNewState() == currentState)
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
                    jumped = false;
                    break;
                case PlayerState.Aerial:
                    airMovement.StartMove();
                    break;
                case PlayerState.OnWall:
                    wallSlide.StartMove();
                    break;
                case PlayerState.Swinging:
                    swing.StartMove();
                    jump.CancelMove();
                    jumped = false;
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

        #region Jump
        private bool ShouldJump => Grounded && playerInputs.TimeSinceJumpWasPressed < settings.jumpBuffer && !jump.IsMoveComplete();
        private bool jumped;
        private void HandleJump()
        {
            if (ShouldJump)
            {
                jump.StartMove();
                jumped = true;
            }
            else
            {
                if (jumped)
                {
                    jump.ContinueMove();
                }
                if (!playerInputs.JumpHeld || jump.IsMoveComplete())
                    ;
                {
                    jump.CancelMove();
                    jumped = false;
                }
            }

        }
        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals("Damages"))
            {
                OnHurt();
            }
        }

        private void OnHurt()
        {
            knockback.StartMove();
        }
        /// <summary>
        /// Respawns the player at a given location.
        /// </summary>
        private void Respawn()
        {
            Debug.Log("Respawn Called");
            if (checkpointManager != null)
            {
                Debug.Log("has Cp manager");
                checkpointManager.RespawnAtRecent(rb.transform);
            }

            rb.velocity = Vector2.zero;
            currentState = PlayerState.Aerial;
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
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(-leftOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.left, filter, hits, sideBounds.x - 0.01f);
            return HitSolidObject(hits);
        }
        public bool TouchingRightWall()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(rightOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.right, filter, hits, sideBounds.x - 0.01f);
            return HitSolidObject(hits);
        }
        public bool TouchingCeiling()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(0, ceilingOffset), new Vector2(ceilingBounds.x, 0.01f), 0, Vector2.up, filter, hits, ceilingBounds.y - 0.01f);
            return HitSolidObject(hits);
        }
        private ContactFilter2D filter;
        public bool TouchingGround()
        {

            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(0, -groundOffset), new Vector2(groundBounds.x, 0.01f), 0, Vector2.down, filter, hits, groundBounds.y - 0.01f);
            return HitSolidObject(hits);

        }

        private static bool HitSolidObject(List<RaycastHit2D> hits)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (!hit.collider.isTrigger)
                {
                    return true;
                }
            }
            return false;
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
