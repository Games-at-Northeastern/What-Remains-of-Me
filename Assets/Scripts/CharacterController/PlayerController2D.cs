using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;
using More2DGizmos;
using System;

namespace PlayerController
{
    /* Script use notes:
     * 
     * - the only direct change to rb.velocity should take place once at then end of a FixedUpdate()
     * 
     * - Never change the rigidbody's position directly, especially after the FlagUpdates() call in a
     * -- FixedUpdate Loop, this may invalidate the script's flags for that update
     * 
     * - When designing behavior in this class which is called during a FixedUpdate, limit uses of External Getters like Grounded which are constant throughout each FixedUpdate
     * -- after FlagUpdates(), but require complex opperations to resolve. Instead, if the value has a defined flag,
     * -- use the flag, as it stores the value from the start of FixedUpdate()
     * 
     * Flag/Feature Section concept:
     * - Most feature sections (Jump, Movement, Swinging, etc.) define some private variables for themselves
     * - Other feature sections as well ControllerCore should not modify these feature variables outside of methods explicilty defined for that purpose, e.g. DeclareJumpInputUsed()
     * 
     * - ControllerCore should instead signal to features using flags. For example, instead of ControllerCore cancelling a Jump directly when it determines that the player landed on the ground,
     * - it enables the fLanded flag, which signals HandleGroundedJump() to cancel any previous jump
     */
    public class PlayerController2D : MonoBehaviour, CharacterController2D
    {
        // Issue: public fields should not exist, and should instead be accessed using an External Getter
        public Vector2 ExternalVelocity; // Vector which external objects (moving platforms, for the most part), modify to change the player's speed
        private Vector2 _speed = Vector2.zero; // speed controlled by actions and inputs native to the player (Basic Ground Movement, Jumping, Swinging, Sliding, etc.)
        [Header("General")]
        [SerializeField] private PlayerState startState = PlayerState.Aerial; // initial player movement state
        public enum PlayerState
        {
            Grounded,
            Aerial,
            Swinging,
            OnWall, // On Wall state is currently never reached
        }
        private PlayerState currentState; // Stores player controller state
        private bool isLocked = false; // when locked, movement input has no effect
        #region Internal References

        [SerializeField] private PlayerSettings stats_; // settings which define player movement
        [SerializeField] private WireThrower wire; // Handles wire and wire physics

        private Rigidbody2D rb; // final value of rb.velocity after each FixedUpdate is ExternalVelocity + _speed
        private ContactFilter2D filter; // this script should only interact with objects of layers included in this filter
        [SerializeField] private CapsuleCollider2D col;

        // IMoves
        private HorizontalSpeed groundMovement; // IMove which modifies _speed.x when the player is Grounded
        private HorizontalSpeed airMovement; // IMove which modifies _speed.x when the player is Aerial
        private VerticalFallSpeed airFall; // not implemented
        private Jump jump; // IMove which modifies _speed.y while the player is jumping
        private Dash dash; // not implemented
        private Swing swing; // Handles player movement while the player is Swinging
        private Knockback knockback; // not implemented
        private WallJump wallJump; // not implemented
        private WallSlide wallSlide; // currently, wallSlide set to have 0 velocity change (effectively disabled)

        private PlayerSFX playerSFX;

        

        [SerializeField] private PlayerInputHandler inputs; /* query this class to recieve player inputs, if locked, inputs will return false for all trigger inputs and 0s for movement input
                                                             * It is PlayerController2d's (this file's) responsiblity to lock and unlock the PlayerInputHandler */

        #region flags
        // flags: true or false values defining events which took place during the physics update between the the former and the current fixedUpdate
        private bool fLanded;
        private bool fLeftGround;
        private bool fSwitchedDirection;

        // flag helpers; useful for determining the value of flags, but can also be used independently throughout the PlayerController
        private float timeSinceLeftGround = Mathf.Infinity;
        private float timeSincePeakJump = 0;
        private float timeSinceLanded = Mathf.Infinity;
        private bool grounded = false;

        private Facing currentDirection;
        #endregion

        private void SetupMoves() // loads stats and this character instance into IMove instances
        {
            groundMovement = new HorizontalSpeed(stats_.maxRunSpeed, stats_.groundedAcceleration, stats_.groundedDeceleration, this);
            airMovement = new HorizontalSpeed(stats_.maxAirSpeed, stats_.airAcceleration, stats_.airDeceleration, this);
            airFall = new VerticalFallSpeed(stats_.terminalVelocity, stats_.fallGravity, this);
            jump = new Jump(stats_.risingGravity, stats_.jumpHeight, JumpType.setSpeed, this);
            dash = new Dash(this, stats_.dashSpeedX, stats_.dashTime);
            swing = new Swing(wire, this, stats_.fallGravity, stats_.wireSwingNaturalAccelMultiplier, stats_.SwingMaxAngularVelocity, stats_.wireSwingDecayMultiplier, stats_.wireSwingBounceDecayMultiplier, stats_.PlayerSwayAccel, stats_.wireLength);
            wallJump = new WallJump(this, stats_.risingGravity, stats_.WallJumpDistance, stats_.takeControlAwayTime);
            wallSlide = new WallSlide(stats_.wallSlideGravity, stats_.maxWallSlideSpeed, this);
        }
        #endregion

        #region External Getters
        public Vector2 Speed { get => _speed; set => _speed = value; }

        public Vector2 Velocity => rb.velocity;

        public Vector3 position => transform.position;

        public bool Grounded => TouchingGround();

        public Vector2 Direction => transform.forward;

        public Facing LeftOrRight => GetNewDirection();
        private Facing GetNewDirection()
        {
            if (inputs.GetMoveInput().x < 0)
            {
                return Facing.left;
            }
            else if (inputs.GetMoveInput().x > 0)
            {
                return Facing.right;
            }
            else
            {
                return currentDirection;
            }
        }
        #endregion


        #region ControllerCore
        private void Start()
        {
            SetupMoves();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CapsuleCollider2D>();
            inputs = GetComponent<PlayerInputHandler>();
            playerSFX = GetComponentInChildren<PlayerSFX>();
            currentState = startState;
            timeSinceJumpWasTriggered = Mathf.Infinity;
            filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(~stats_.IgnoreLayers); // sets the layer mask to all layers which shouldnt be ignored. "~" inverts the IgnoreLayers flags
            LevelManager.OnPlayerReset.AddListener(Respawn);
            LevelManager.OnPlayerDeath.AddListener(Restart);

            ResetFlags();
        }
        private void OnValidate()
        {
            filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(~stats_.IgnoreLayers);
        }
        private void FixedUpdate()
        {
            FlagUpdates();
            HandleSoundTriggers();
            SwitchState();
            switch (currentState)
            {
                case PlayerState.Grounded:
                    HandleGroundHorizontal();
                    HandleGroundedJump();
                    HandleVerticalGrounded();
                    break;
                case PlayerState.Aerial:
                    HandleAirHorizontal();
                    HandleVerticalAerial();
                    break;
                case PlayerState.Swinging:
                    if (JumpBuffered)
                    {
                        swing.CancelMove();
                        DeclareJumpInputUsed();
                        wire.DisconnectWire();
                    }
                    else
                    {
                        swing.UpdateInput(inputs.GetMoveInput().x);
                        swing.ContinueMove();
                    }
                    break;
            }
            rb.velocity = _speed + ExternalVelocity;
            HandleExternalVelocityDecay();
        }

        /// <summary>
        /// Used to slow down the player's ExternalVelocity once they are no longer affected by an external object
        /// </summary>
        private void HandleExternalVelocityDecay()
        {
            ExternalVelocity.x = Mathf.MoveTowards(ExternalVelocity.x, 0, stats_.ExternalVelocityDecay * Time.fixedDeltaTime);
            if (ExternalVelocity.y < 0 && !grounded)
            {

                _speed.y = Mathf.Clamp(rb.velocity.y, stats_.terminalVelocity, 0);
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
                    swing.UpdateInput(inputs.GetMoveInput().x);
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
        private PlayerState GetNewState()
        {
            if (grounded)
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
        #endregion

        /// <summary>
        /// Updates the flags and flag helpers based on changes during the physics update after the previous FixedUpdate
        /// </summary>
        /// <returns></returns>
        protected virtual void FlagUpdates()
        {
            ResetFlags();

            bool lastGroundedState = grounded;
            grounded = TouchingGround();


            if (!lastGroundedState && grounded) // Landed
            {
                timeSinceLanded = 0;
                fLanded = true;
            }
            else if (lastGroundedState && !grounded) // Left the ground
            {
                fLeftGround = true;
                timeSinceLeftGround = 0;
                timeSincePeakJump = 0;
            }
            else if (!grounded) // Remained non grounded
            {
                timeSinceLeftGround += Time.fixedDeltaTime;

                if (rb.velocity.y < 0)
                {
                    timeSincePeakJump += Time.fixedDeltaTime;
                }
            }
            else // Remained grounded
            {
                timeSinceLanded += Time.fixedDeltaTime;
            }
            timeSinceJumpWasTriggered += Time.fixedDeltaTime;

            Facing lastDirection = currentDirection;
            currentDirection = GetNewDirection();

            if (lastDirection != currentDirection)
            {
                fSwitchedDirection = true;
            }
        }

        /// <summary>
        /// Resets event flags to their default state (false)
        /// </summary>
        /// <returns></returns>
        protected virtual void ResetFlags()
        {
            fLanded = false;
            fLeftGround = false;
            fSwitchedDirection = false;
        }

        /// <summary>
        /// Handles sounds caused by flags
        /// </summary>
        private void HandleSoundTriggers()
        {
            if (fLanded)
            {
                PlayLandingNoise();
            }
        }

        #endregion

        #region Grounded and Aerial

        

        /// <summary>
        /// Handles _speed.y when the player is grounded
        /// </summary>
        protected virtual void HandleVerticalGrounded()
        {
            if (fLanded) // On landing, sets _speed.y to a small negative value and resets ExternalVelocity
            {
                _speed.y = -1 * Time.fixedDeltaTime;
                if (ExternalVelocity.y > 0)
                {
                    ExternalVelocity.y = 0;
                }
            }
        }

        /// <summary>
        /// A helper function that uses _speed.x to stop horizontal player momentum in the direction of the wall the player is touching.
        /// </summary>
        private void IfTouchingWallThenStopMomentum()
        {
            if (TouchingLeftWall() && _speed.x < 0)
            {
                _speed.x = 0;
            }
            if (TouchingRightWall() && _speed.x > 0)
            {
                _speed.x = 0;
            }
        }

        /// <summary>
        /// Handles horizontal movement when the player is grounded
        /// </summary>
        protected virtual void HandleGroundHorizontal()
        {
            IfTouchingWallThenStopMomentum();
            groundMovement.UpdateDirection(inputs.GetMoveInput().x);
            
            groundMovement.ContinueMove();
        }

        /// <summary>
        /// Handles horizontal movement when the player is aerial
        /// </summary>
        protected virtual void HandleAirHorizontal()
        {
            IfTouchingWallThenStopMomentum();
            airMovement.UpdateDirection(inputs.GetMoveInput().x);
            airMovement.ContinueMove();
        }

        /// <summary>
        /// Handles vertical movement when the player is aerial
        /// </summary>
        protected virtual void HandleVerticalAerial()
        {
            if (TouchingCeiling())
            {
                Speed = new Vector2(Speed.x, 0);
                CancelJump();
            }
            if ((!inputs.IsJumpHeld() && !jumpCanceled) || (!jumpCanceled
                && jump.IsMoveComplete()))
            {
                CancelJump();
            }
            if (!jump.IsMoveComplete())
            {
                jump.ContinueMove();
            }

            if (!grounded /*(timeSinceLeftGround > _stats.coyoteTime)*/) // apply gravity (uncomment the coyote time section if you dont want the player to fall while they can still coyote jump)
            {
                float gravityDelta = stats_.fallGravity * Time.deltaTime;
                if(ExternalVelocity.y > 0)
                {
                    ExternalVelocity.y += gravityDelta;
                    if(ExternalVelocity.y < 0)
                    {
                        gravityDelta = ExternalVelocity.y;
                        ExternalVelocity.y = 0;

                    }
                }

                _speed.y = Mathf.Max(stats_.terminalVelocity, Speed.y + gravityDelta);
            }
        }
        #endregion

        #region Jump
        private float timeSinceJumpWasTriggered;
        private bool jumpCanceled = true;

        // if a jump is buffered to be initiated
        private bool JumpBuffered => inputs.TimeSinceJumpWasPressed < stats_.jumpBuffer && !ThisJumpInputWasUsed;
        // if the most recent jump input has triggered a jump
        private bool ThisJumpInputWasUsed => timeSinceJumpWasTriggered <= inputs.TimeSinceJumpWasPressed;
        protected virtual bool ShouldTriggerJump() => CanCoyoteJump() || CanGroundedJump();
        // if the player can "coyote jump", meaning they can trigger a jump during a small set time period after they leave the ground without jumping
        protected virtual bool CanCoyoteJump() => (timeSinceLeftGround < stats_.coyoteTime)
                && JumpBuffered && jumpCanceled; // Checks JumpCanceled to stop situation where player triggers a grounded and coyote jump by spamming
        // declares that a jump input was used without actually triggering a jump
        protected virtual void DeclareJumpInputUsed() => timeSinceJumpWasTriggered = 0;

        protected virtual void PlayLandingNoise()
        {
            if (timeSincePeakJump > 0)
            {
                playerSFX.JumpLand(Math.Min(2f, timeSincePeakJump / stats_.landingVolumeTime));
            }
        }

        protected virtual bool CanGroundedJump() => grounded && JumpBuffered;

        protected virtual void HandleGroundedJump()
        {
            if (fLanded && !jumpCanceled)
            {
                CancelJump();
            }
            if (ShouldTriggerJump())
            {
                TriggerJump();
            }
        }
        protected virtual void CancelJump()
        {
            jump.CancelMove();
            jumpCanceled = true;
        }
        protected virtual void TriggerJump()
        {
            if (ExternalVelocity.y < 0)
            {
                ExternalVelocity.y = 0;
            }
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
            Physics2D.BoxCast(origin + new Vector2(-stats_.leftOffset, 0), new Vector2(0.01f, stats_.sideBounds.y), 0, Vector2.left, filter, hits, stats_.sideBounds.x - 0.01f);
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
            Physics2D.BoxCast(origin + new Vector2(stats_.rightOffset, 0), new Vector2(0.01f, stats_.sideBounds.y), 0, Vector2.right, filter, hits, stats_.sideBounds.x - 0.01f);
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
            Physics2D.BoxCast(origin + new Vector2(0, stats_.ceilingOffset), new Vector2(stats_.ceilingBounds.x, 0.01f), 0, Vector2.up, filter, hits, stats_.ceilingBounds.y - 0.01f);
            return HitSolidObject(hits);
        }

        /// <summary>
        /// Is player touching the ground
        /// </summary>
        /// <returns></returns>
        public bool TouchingGround()
        {

            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            Physics2D.BoxCast(origin + new Vector2(0, -stats_.groundOffset), new Vector2(stats_.groundBounds.x, 0.02f), 0, Vector2.down, filter, hits, 0.05f);
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
        // not yet implemented
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
            LevelManager.checkpointManager.RespawnAtRecent(rb.transform);
            rb.velocity = Vector2.zero;
            currentState = PlayerState.Aerial;
            ExternalVelocity = Vector2.zero;
            _speed = Vector2.zero;
        }
        /// <summary>
        /// Resets the player to their original position. For debugging only.
        /// </summary>
        private void Restart()
        {
            LevelManager.checkpointManager.RespawnAtBeginning(rb.transform);
            rb.velocity = Vector2.zero;
            ExternalVelocity = Vector2.zero;
            currentState = PlayerState.Aerial;
            _speed = Vector2.zero;
        }

        #endregion

        #region Conditions

        // Sets this scripts isLocked value to true and locks the PlayerInputHandler
        public void LockInputs()
        {
            isLocked = true;
            inputs.Lock();
        }

        // Sets this scripts isLocked value to false and unlocks the PlayerInputHandler
        public void UnlockInputs()
        {
            isLocked = false;
            inputs.Unlock();
        }
        public bool LockedOrNot() => isLocked;

        // Calculates and returns Atlas's current animation state
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
            GizmosPlus.BoxCast(origin + new Vector2(stats_.rightOffset, 0), new Vector2(0.01f, stats_.sideBounds.y), 0, Vector2.right, stats_.sideBounds.x - 0.01f, ~stats_.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(-stats_.leftOffset, 0), new Vector2(0.01f, stats_.sideBounds.y), 0, Vector2.left, stats_.sideBounds.x - 0.01f, ~stats_.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(0, stats_.ceilingOffset), new Vector2(stats_.ceilingBounds.x, 0.01f), 0, Vector2.up, stats_.ceilingBounds.y - 0.01f, ~stats_.IgnoreLayers);
            GizmosPlus.BoxCast(origin + new Vector2(0, -stats_.groundOffset), new Vector2(stats_.groundBounds.x, 0.01f), 0, Vector2.down, 0.05f, ~stats_.IgnoreLayers);
        }
#endif

    }

}
