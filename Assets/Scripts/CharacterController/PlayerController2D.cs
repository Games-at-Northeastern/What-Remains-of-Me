using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;
using More2DGizmos;
using System;

namespace PlayerController
{
    public class PlayerController2D : MonoBehaviour, CharacterController2D
    {
        public Vector2 ExternalVelocity;
        [Header("General")]
        public PlayerState startState = PlayerState.Aerial;
        public enum PlayerState
        {
            Grounded,
            Aerial,
            Swinging,
            OnWall,
        }
        private PlayerState currentState = PlayerState.Aerial;
        private bool isLocked = false;
        #region Internal References

        public PlayerSettings _stats;
        public WireThrower wire;
        [HideInInspector] Rigidbody2D _rb;
        [HideInInspector] public CapsuleCollider2D col;
        HorizontalSpeed groundMovement;
        HorizontalSpeed airMovement;
        VerticalFallSpeed airFall;
        Jump jump;
        Dash dash;
        Swing swing;
        Knockback knockback;
        WallJump wallJump;
        WallSlide wallSlide;
        PlayerSFX playerSFX;

        PlayerInputHandler inputs;
        private Vector2 _speed = Vector2.zero;

        private void SetupMoves()
        {
            groundMovement = new HorizontalSpeed(_stats.maxRunSpeed, _stats.groundedAcceleration, _stats.groundedDeceleration, this);
            airMovement = new HorizontalSpeed(_stats.maxAirSpeed, _stats.airAcceleration, _stats.airDeceleration, this);
            airFall = new VerticalFallSpeed(_stats.terminalVelocity, _stats.fallGravity, this);
            jump = new Jump(_stats.risingGravity, _stats.jumpHeight, JumpType.setSpeed, this);
            dash = new Dash(this, _stats.dashSpeedX, _stats.dashTime);
            swing = new Swing(wire, this, _stats.fallGravity, _stats.wireSwingNaturalAccelMultiplier, _stats.SwingMaxAngularVelocity, _stats.wireSwingDecayMultiplier, _stats.wireSwingBounceDecayMultiplier, _stats.PlayerSwayAccel, _stats.wireLength);
            wallJump = new WallJump(this, _stats.risingGravity, _stats.WallJumpDistance, _stats.takeControlAwayTime);
            wallSlide = new WallSlide(_stats.wallSlideGravity, _stats.maxWallSlideSpeed, this);
        }
        #endregion

        #region External
        public Vector2 Speed { get => _speed; set => _speed = value; }

        public Vector2 Velocity => _rb.velocity;

        public Vector3 position => transform.position;

        public bool Grounded => TouchingGround();

        public Vector2 Direction => transform.forward;

        public Facing LeftOrRight => direction();
        private Facing currentDirection = Facing.right;
        private Facing direction()
        {
            if (inputs.MoveInput.x < 0 && !isLocked)
            {
                currentDirection = Facing.left;
                return Facing.left;
            }
            else if (inputs.MoveInput.x > 0 && !isLocked)
            {
                currentDirection = Facing.right;
                return Facing.right;
            }
            else
                return currentDirection;
        }
        #endregion


        #region ControllerCore
        private void Start()
        {
            SetupMoves();
            _rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CapsuleCollider2D>();
            inputs = GetComponent<PlayerInputHandler>();
            playerSFX = GetComponentInChildren<PlayerSFX>();
            currentState = startState;
            timeSinceJumpWasTriggered = Mathf.Infinity;
            jumped = true;
            filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(~_stats.IgnoreLayers);
            LevelManager.OnPlayerReset.AddListener(Respawn);
            LevelManager.OnPlayerDeath.AddListener(Restart);
        }
        private void OnValidate()
        {
            filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(~_stats.IgnoreLayers);
        }
        private void FixedUpdate()
        {
            FlagUpdates();
            SwitchState();
            switch (currentState)
            {
                case PlayerState.Grounded:
                    HandleGroundHorizontal();
                    HandleGroundedJump();
                    break;
                case PlayerState.Aerial:
                    HandleAirHorizontal();
                    HandleVertical();
                    HandleGroundedJump();
                    break;
                case PlayerState.Swinging:
                    if (JumpBuffered)
                    {
                        swing.CancelMove();
                        timeSinceJumpWasTriggered = 0;
                        wire.DisconnectWire();
                    }
                    else
                    {
                        swing.UpdateInput(inputs.MoveInput.x);
                        swing.ContinueMove();
                    }
                    break;
            }
            _rb.velocity = _speed + ExternalVelocity;
            HandleExternalVelocityDecay();
        }

        private void HandleExternalVelocityDecay()
        {
            ExternalVelocity.x = Mathf.MoveTowards(ExternalVelocity.x, 0, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);
            if (ExternalVelocity.y < 0 && !Grounded)
            {

                _speed.y = Mathf.Clamp(_rb.velocity.y, _stats.terminalVelocity, 0);
                ExternalVelocity.y = 0;
            }
        }

        #region StateTransition
        /// <summary>
        /// Switches the current player state to the new state.
        /// if state switches performs startMove methods.
        /// </summary>
        private void SwitchState()
        {
            PlayerState newState = GetNewState();
            if (newState == currentState)
            {
                return;
            }
            else
            {
                if (currentState == PlayerState.Swinging)
                {
                    swing.CancelMove();
                }
                currentState = newState;
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
                    swing.UpdateInput(inputs.MoveInput.x);
                    swing.StartMove();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Returns the state the player should switch to
        /// </summary>
        /// <returns></returns>
        PlayerState GetNewState()
        {
            if (Grounded)
            {
                return PlayerState.Grounded;
            }
            else
            {
                if (wire.IsConnected() && jumpCanceled)
                {
                    return PlayerState.Swinging;
                }
                else
                {
                    return PlayerState.Aerial;
                }
            }
        }

        bool CheckGrounded()
        {
            return TouchingGround();
        }
        #endregion
        private float TimeSinceLeftGround = Mathf.Infinity;
        private float TimeSincePeakJump = 0;
        private float TimeSinceLanded = Mathf.Infinity;
        private bool _grounded = false;
        protected virtual void FlagUpdates()
        {
            bool lastGroundedState = _grounded;
            _grounded = CheckGrounded();
            if (!lastGroundedState && _grounded)
            {
                TimeSinceLanded = 0;
                PlayLandingNoise();
            }
            else if (lastGroundedState && !_grounded)
            {
                TimeSinceLeftGround = 0;
                TimeSincePeakJump = 0;
            }
            else if (!_grounded)
            {
                TimeSinceLeftGround += Time.fixedDeltaTime;

                if (_rb.velocity.y < 0)
                {
                    TimeSincePeakJump += Time.fixedDeltaTime;
                }
            }
            else
            {
                TimeSinceLanded += Time.fixedDeltaTime;
            }
            if (TimeSinceLanded == 0 && Grounded)
            {
                _speed.y = -1 * Time.fixedDeltaTime;
                jumpCanceled = true;
                if(ExternalVelocity.y > 0)
                {
                    ExternalVelocity.y = 0;
                }
            }
            timeSinceJumpWasTriggered += Time.fixedDeltaTime;
        }

        #endregion

        #region Grounded and Aerial
        protected virtual void HandleAirHorizontal()
        {
            if (!isLocked)
            {
                airMovement.UpdateDirection(inputs.MoveInput.x);
            }
            else
            {
                airMovement.UpdateDirection(0);
            }
            airMovement.ContinueMove();
        }
        protected virtual void HandleGroundHorizontal()
        {
            if (!isLocked)
            {
                groundMovement.UpdateDirection(inputs.MoveInput.x);
            }
            else
            {
                groundMovement.UpdateDirection(0);
            }
            groundMovement.ContinueMove();
        }

        protected virtual void HandleVertical()
        {
            if (TouchingCeiling())
            {
                jump.CancelMove();
            }
            if (!jump.IsMoveComplete())
            {
                jump.ContinueMove();
            }

            if (!Grounded && (TimeSinceLeftGround > _stats.coyoteTime || jumped))
            {
                float gravityDelta = _stats.fallGravity * Time.deltaTime;
                if(ExternalVelocity.y > 0)
                {
                    ExternalVelocity.y += gravityDelta;
                    if(ExternalVelocity.y < 0)
                    {
                        gravityDelta = ExternalVelocity.y;
                        ExternalVelocity.y = 0;

                    }
                }

                _speed.y = Mathf.Max(_stats.terminalVelocity, Speed.y + gravityDelta);
            }
        }
        #endregion

        #region Jump
        public float timeSinceJumpWasTriggered;
        public bool jumpCanceled = false;
        public bool jumped;

        private bool JumpBuffered => inputs.TimeSinceJumpWasPressed < _stats.jumpBuffer && !ThisJumpInputWasUsed;
        private bool ThisJumpInputWasUsed => timeSinceJumpWasTriggered <= inputs.TimeSinceJumpWasPressed;
        protected virtual bool ShouldTriggerJump()
        {
            return CanCoyoteJump() || CanGroundedJump();
        }
        protected virtual bool CanCoyoteJump()
        {
            return (TimeSinceLeftGround < _stats.coyoteTime)
                && (JumpBuffered);
        }

        protected virtual void PlayLandingNoise()
        {
            if (TimeSincePeakJump > 0)
            {
                playerSFX.JumpLand(Math.Min(2f, TimeSincePeakJump / _stats.landingVolumeTime));
            }
        }

        protected virtual bool CanGroundedJump()
        {
            return Grounded && JumpBuffered && !isLocked;
        }

        protected virtual void HandleGroundedJump()
        {
            if (ShouldTriggerJump())
            {
                TriggerJump();
            }
            if ((!inputs.JumpHeld && !jumpCanceled) || (!jumpCanceled
                && jump.IsMoveComplete()))
            {
                CancelJump();
            }
        }
        protected virtual void CancelJump()
        {
            jump.CancelMove();
            jumpCanceled = true;
        }
        protected virtual void TriggerJump()
        {
            jump.StartMove();
            jumpCanceled = false;
            timeSinceJumpWasTriggered = 0;
        }
        #endregion Jump

        #region Collision

        /// <summary>
        /// Is player touching wall to their left
        /// </summary>
        /// <returns></returns>
        public bool TouchingLeftWall()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(-_stats.leftOffset, 0), new Vector2(0.01f, _stats.sideBounds.y), 0, Vector2.left, filter, hits, _stats.sideBounds.x - 0.01f);
            return HitSolidObject(hits);
        }
        /// <summary>
        /// Is player touching wall to their right
        /// </summary>
        /// <returns></returns>
        public bool TouchingRightWall()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(_stats.rightOffset, 0), new Vector2(0.01f, _stats.sideBounds.y), 0, Vector2.right, filter, hits, _stats.sideBounds.x - 0.01f);
            return HitSolidObject(hits);
        }
        /// <summary>
        /// Is player touching the ceiling?
        /// </summary>
        /// <returns></returns>
        public bool TouchingCeiling()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(0, _stats.ceilingOffset), new Vector2(_stats.ceilingBounds.x, 0.01f), 0, Vector2.up, filter, hits, _stats.ceilingBounds.y - 0.01f);
            return HitSolidObject(hits);
        }
        private ContactFilter2D filter;
        /// <summary>
        /// Is player touching the ground
        /// </summary>
        /// <returns></returns>
        public bool TouchingGround()
        {

            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(0, -_stats.groundOffset), new Vector2(_stats.groundBounds.x, 0.02f), 0, Vector2.down, filter, hits, 0.05f);
            return HitSolidObject(hits);

        }
        /// <summary>
        /// Did raycast hit a solid object
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //calls the player to do what they need to take damage
            if (collision.gameObject.layer.Equals("Damages"))
            {
                OnHurt();
            }
        }
        private void OnHurt()
        {
            knockback.StartMove();
        }
        #endregion

        #region Death

        /// <summary>
        /// Respawns the player at a given location.
        /// </summary>
        private void Respawn()
        {
            LevelManager.checkpointManager.RespawnAtRecent(_rb.transform);
            _rb.velocity = Vector2.zero;
            currentState = PlayerState.Aerial;
            ExternalVelocity = Vector2.zero;
            _speed = Vector2.zero;
        }
        /// <summary>
        /// Resets the player to their original position. For debugging only.
        /// </summary>
        private void Restart()
        {
            LevelManager.checkpointManager.RespawnAtBeginning(_rb.transform);
            _rb.velocity = Vector2.zero;
            ExternalVelocity = Vector2.zero;
            currentState = PlayerState.Aerial;
            _speed = Vector2.zero;
        }

        #endregion

        #region Conditions

        public void LockInputs()
        {
            isLocked = true;
        }
        public void UnlockInputs()
        {
            isLocked = false;
        }
        public bool LockedOrNot() 
        {
            return isLocked;
        }
        public AnimationType GetAnimationState()
        {
            switch (currentState)
            {
                case PlayerState.Grounded:
                    if (_speed.x != 0)
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
        #endregion


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            GizmosPlus.BoxCast(origin + new Vector2(_stats.rightOffset, 0), new Vector2(0.01f, _stats.sideBounds.y), 0, Vector2.right, _stats.sideBounds.x - 0.01f, ~_stats.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(-_stats.leftOffset, 0), new Vector2(0.01f, _stats.sideBounds.y), 0, Vector2.left, _stats.sideBounds.x - 0.01f, ~_stats.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(0, _stats.ceilingOffset), new Vector2(_stats.ceilingBounds.x, 0.01f), 0, Vector2.up, _stats.ceilingBounds.y - 0.01f, ~_stats.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(0, -_stats.groundOffset), new Vector2(_stats.groundBounds.x, 0.01f), 0, Vector2.down, 0.05f, ~_stats.IgnoreLayers);
        }
#endif

    }

}
