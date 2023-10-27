namespace CharacterController
{
    using UnityEngine;

    /// <summary>
    /// A move that exerts knockback to a character
    /// </summary>
    public class Knockback : IMove
    {


        private Vector2 vel;
        private ICharacterController character;
        private readonly bool unlockOnGrounded;
        private readonly float lockOutTime;
        private float timePassed;
        private readonly Vector2 acceleration;
        private readonly float maxFallSpeed;
        /// <summary>
        /// Initializes a knockback move, starting the player at a specific vertical
        /// and horizontal speed.
        /// when x and y in accelerations is positive x will decelerate to 0 and y wil got accelerate to -maxFallSpeed
        /// </summary>
        public Knockback(Vector2 direction, float magnitude,Vector2 acceleration, float maxFallSpeed, bool unlockOnGrounded, float lockOutTime, ICharacterController character)
        {
            this.unlockOnGrounded = unlockOnGrounded;
            vel = direction.normalized * magnitude;
            this.character = character;
            this.acceleration = acceleration;
            this.maxFallSpeed = maxFallSpeed;
        }

        public AnimationType GetAnimationState() => AnimationType.KNOCKBACK;
        public void StartMove() => character.Speed = vel;
        public void ContinueMove()
        {
            timePassed += Time.fixedDeltaTime;
            character.Speed.Set(Mathf.MoveTowards(character.Speed.x, 0, acceleration.x), Mathf.MoveTowards(character.Speed.y,maxFallSpeed, acceleration.y));
        }
        public void CancelMove() { }
        public bool IsMoveComplete()
        {
            if (unlockOnGrounded)
            {
                if (character.Grounded)
                {
                    return true;
                }
            }
            if(timePassed >= lockOutTime)
            {
                return true;
            }
            return false;
        }
    }
}
