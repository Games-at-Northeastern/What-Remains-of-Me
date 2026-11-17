using UnityEngine;

namespace PlayerController
{
    /// <summary>
    /// One mechanic of the player's movement. Added as a component next to PlayerPhysicsController2D, it is picked up
    /// automatically and run every physics step in ascending Order while new physics is active.
    /// </summary>
    [RequireComponent(typeof(PlayerPhysicsController2D))]
    public abstract class PlayerPhysicsModule : MonoBehaviour
    {
        public abstract int Order { get; }

        protected PlayerPhysicsController2D Player { get; private set; }

        public void Initialize(PlayerPhysicsController2D player)
        {
            Player = player;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Changes the movement for this physics step. Returning true takes over the step and skips the modules after
        /// this one.
        /// </summary>
        public abstract bool UpdateMovement(PlayerPhysicsContext context);

        /// <summary>
        /// Runs on every enabled module after UpdateMovement, even when another module took over the step.
        /// </summary>
        public virtual void ConstrainMovement(PlayerPhysicsContext context)
        {
        }

        public virtual void ResetState()
        {
        }

        /// <summary>
        /// Called when the player switches to the original PlayerController2D movement.
        /// </summary>
        public virtual void OnNewPhysicsDisabled()
        {
        }

        protected virtual void OnDisable() => ResetState();
    }
}
