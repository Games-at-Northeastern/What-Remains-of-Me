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
        [Header("General")]
        public PlayerState startState = PlayerState.Aerial;
        public Vector2 ExternalVelocity;
        public enum PlayerState
        {
            Grounded,
            Aerial,
            Swinging,
            OnWall,
        }
        private PlayerState currentState = PlayerState.Aerial;
        private float currentGravity;
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

        PlayerInputHandler inputs;
        private Vector2 _speed = Vector2.zero;


        #endregion

        #region external
        public Vector2 Speed => _rb.velocity;
        #endregion


        #region ControllerCore
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CapsuleCollider2D>();
            inputs = GetComponent<PlayerInputHandler>();
            currentState = startState;
            currentGravity = _stats.fallGravity;
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

            _speed = FrameStartingSpeed();
            FlagUpdates();
            SwitchState();
            switch (currentState)
            {
                case PlayerState.Grounded:
                    HandleSpeed();
                    HandleGroundedJump();
                    break;
                case PlayerState.Aerial:
                    HandleSpeed();
                    HandleGroundedJump();
                    break;
            }
         
            _rb.velocity = _speed + ExternalVelocity;
            HandleExternalVelocityDecay();
        }
        private Vector2 FrameStartingSpeed()
        {
            Vector2 _speed = Vector2.zero;
            if (_rb.velocity.x > 0)
            {
                _speed.x = _rb.velocity.x - ExternalVelocity.x;
                if (_speed.x < 0)
                {
                    _speed.x = _rb.velocity.x;
                }

            }
            else
            {
                _speed.x = _rb.velocity.x + ExternalVelocity.x;
                if (_speed.x > 0)
                {
                    _speed.x = _rb.velocity.x;
                }
            }
            if (_rb.velocity.y > 0)
            {
                _speed.y = _rb.velocity.y - ExternalVelocity.y;
                if (_speed.y < 0)
                {
                    _speed.x = _rb.velocity.x;
                }

            }
            else
            {
                _speed.y = _rb.velocity.y + ExternalVelocity.y;
                if (_speed.x > 0)
                {
                    _speed.y = _rb.velocity.y;
                }
            }
            return _rb.velocity - ExternalVelocity;
 
        }
        private void HandleExternalVelocityDecay()
        {
            ExternalVelocity.x = Mathf.MoveTowards(ExternalVelocity.x, 0, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);
            if (ExternalVelocity.y < 0)
            {
                ExternalVelocity.y = Mathf.MoveTowards(ExternalVelocity.y, 0, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);
            }
            else
            {
                ExternalVelocity.y = Mathf.MoveTowards(ExternalVelocity.y, 0, Mathf.Abs(_stats.fallGravity) * Time.fixedDeltaTime);
            }
        }

        #region StateTransition
        /// <summary>
        /// Switches the current player state to the new state.
        /// if state switches performs startMove methods.
        /// </summary>
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
                    //groundMovement.StartMove();
                    break;
                case PlayerState.Aerial:
                    //airMovement.StartMove();
                    break;
                case PlayerState.OnWall:
                    //wallSlide.StartMove();
                    break;
                case PlayerState.Swinging:
                    //swing.StartMove();
                    //jump.CancelMove();
                    //jumped = false;
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
                if (((wire.IsConnected() && Velocity.y < 0) || (PlayerState.Swinging == currentState && wire.IsConnected())))
                {
                    //return PlayerState.Swinging;
                    return PlayerState.Aerial;
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
        private float TimeSinceLanded = Mathf.Infinity;
        private bool _grounded = false;
        protected virtual void FlagUpdates()
        {
            bool lastGroundedState = _grounded;
            _grounded = CheckGrounded();
            if (!lastGroundedState && _grounded)
            {
                TimeSinceLanded = 0;
            }
            else if (lastGroundedState && !_grounded)
            {
                TimeSinceLeftGround = 0;
            }
            else if (!_grounded)
            {
                TimeSinceLeftGround += Time.fixedDeltaTime;

            }
            else
            {
                TimeSinceLanded += Time.fixedDeltaTime;
            }
            if (TimeSinceLanded == 0 && Grounded)
            {
                currentGravity = _stats.fallGravity;
                _speed.y = 0;
                ExternalVelocity.y = 0;
                jumpCanceled = true;
            }
            timeSinceJumpWasTriggered += Time.fixedDeltaTime;
        }

        #endregion

        #region Grounded and Aerial
        protected virtual void HandleSpeed()
        {
            HandleHorizontal();
            HandleVertical();
        }
        protected virtual void HandleHorizontal()
        {
            float HI = inputs.MoveInput.x;
            if (Grounded)
            {
                //Acceleration to max speed

                //Deceleration;
                if (HI == 0)
                {
                    _speed.x = Kinematics.VelocityTowards(_speed.x, _stats.groundedDeceleration, 0, Time.fixedDeltaTime);
                }
                else
                {
                    _speed.x = Kinematics.VelocityTowards(_speed.x, _stats.groundedAcceleration, HI * _stats.maxRunSpeed, Time.fixedDeltaTime);
                }
            }
            else
            {

                //Deceleration;
                if (HI == 0)
                {
                    _speed.x = Kinematics.VelocityTowards(_speed.x, _stats.airDeceleration, 0, Time.fixedDeltaTime);
                }
                else
                {
                    _speed.x = Kinematics.VelocityTowards(_speed.x, _stats.airAcceleration, HI * _stats.maxRunSpeed, Time.fixedDeltaTime);
                }
            }


        }

        protected virtual void HandleVertical()
        {
            if (!Grounded && (TimeSinceLeftGround > _stats.coyoteTime || jumped))
            {
                _speed.y = Kinematics.VelocityTarget(_speed.y, currentGravity, _stats.terminalVelocity, Time.fixedDeltaTime);
            }
        }
        #endregion

        #region Jump
        public float timeSinceJumpWasTriggered;
        public bool jumpCanceled = false;
        public float timeSinceJumpWasInputed => inputs.TimeSinceJumpWasPressed;
        public bool jumped;

        private bool JumpBuffered => timeSinceJumpWasInputed < _stats.jumpBuffer && !ThisJumpInputWasUsed;
        private bool ThisJumpInputWasUsed => timeSinceJumpWasTriggered <= timeSinceJumpWasInputed;
        protected virtual bool ShouldTriggerJump()
        {
            return CanCoyoteJump() || CanGroundedJump();
        }
        protected virtual bool CanCoyoteJump()
        {
            return (TimeSinceLeftGround < _stats.coyoteTime)
                && (JumpBuffered);
        }

        protected virtual bool CanGroundedJump()
        {
            return Grounded && JumpBuffered;
        }

        protected virtual void HandleGroundedJump()
        {
            if (ShouldTriggerJump())
            {

                //Debug.Log("TriggeredJump");
                TriggerJump();

            }
            Debug.Log("jumpheld" + inputs.JumpHeld + " ShouldCancel" + (!inputs.JumpHeld && !jumpCanceled)); //|| (!jumpCanceled
               // && (Kinematics.InitialVelocity(0, _stats.risingGravity, _stats.jumpHeight) / -_stats.risingGravity) < timeSinceJumpWasTriggered)));
            if ((!inputs.JumpHeld && !jumpCanceled) || (!jumpCanceled
                && (Kinematics.InitialVelocity(0, _stats.risingGravity, _stats.jumpHeight) / -_stats.risingGravity) < timeSinceJumpWasTriggered))
            {
                CancelJump();
            }
        }
        protected virtual void CancelJump()
        {
            currentGravity = _stats.fallGravity;
            _speed.y *= _stats.jumpEndedEarlyMultiplier;
            jumpCanceled = true;
        }
        protected virtual void TriggerJump()
        {
            currentGravity = _stats.risingGravity;
            jumpCanceled = false;
            _speed.y = Kinematics.InitialVelocity(0, _stats.risingGravity, _stats.jumpHeight);
            timeSinceJumpWasTriggered = 0;
        }
        #endregion Jump

        Vector2 CharacterController2D.Speed { get => _speed; set => _speed = value; }

        public Vector2 Velocity => _rb.velocity;

        public Vector3 position => transform.position;

        public bool Grounded => TouchingGround();

        public Vector2 Direction => transform.forward;

        public Facing LeftOrRight => direction();
        private Facing currentDirection = Facing.right;
        private Facing direction()
        {
            if (inputs.MoveInput.x == 0)
            {
                return currentDirection;
            }
            if (inputs.MoveInput.x < 0)
            {
                currentDirection = Facing.left;
                return Facing.left;
            }
            currentDirection = Facing.right;
            return Facing.right;
        }


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
            Physics2D.BoxCast(origin + new Vector2(0, -_stats.groundOffset), new Vector2(_stats.groundBounds.x, 0.01f), 0, Vector2.down, filter, hits, _stats.groundBounds.y - 0.01f);
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

        /// <summary>
        /// Respawns the player at a given location.
        /// </summary>
        private void Respawn()
        {
            LevelManager.checkpointManager.RespawnAtRecent(_rb.transform);
            _rb.velocity = Vector2.zero;
            currentState = PlayerState.Aerial;
            ExternalVelocity = Vector2.zero;
        }
        /// <summary>
        /// Resets the player to their original position. For debugging only.
        /// </summary>
        private void Restart()
        {
            LevelManager.checkpointManager.RespawnAtBeginning(_rb.transform);
            _rb.velocity = Vector2.zero;
            currentState = PlayerState.Aerial;
            ExternalVelocity = Vector2.zero;
        }


        #region Conditions

        public void LockInputs() => throw new NotImplementedException();
        public void UnlockInputs() => throw new NotImplementedException();
        public AnimationType GetAnimationState()
        {
            switch (currentState)
            {
                case PlayerState.Grounded:
                    if (Mathf.Abs(inputs.MoveInput.x) > 0.1f)
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



        private void OnDrawGizmos()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            GizmosPlus.BoxCast(origin + new Vector2(_stats.rightOffset, 0), new Vector2(0.01f, _stats.sideBounds.y), 0, Vector2.right, _stats.sideBounds.x - 0.01f, ~_stats.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(-_stats.leftOffset, 0), new Vector2(0.01f, _stats.sideBounds.y), 0, Vector2.left, _stats.sideBounds.x - 0.01f, ~_stats.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(0, _stats.ceilingOffset), new Vector2(_stats.ceilingBounds.x, 0.01f), 0, Vector2.up, _stats.ceilingBounds.y - 0.01f, ~_stats.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(0, -_stats.groundOffset), new Vector2(_stats.groundBounds.x, 0.01f), 0, Vector2.down, _stats.groundBounds.y - 0.01f, ~_stats.IgnoreLayers);
        }


    }

}
