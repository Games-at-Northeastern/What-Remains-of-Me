using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    /// Represents a move in which the player dashes, on air or on ground,
    /// at a quick speed.
    /// </summary>
    public class Dash : IMove
    {
        private float timePassed;
        private float dashSpeed;
        private float dashTime;
        private ICharacterController character;
        /// <summary>
        /// Initializes a dash, taking in whether it is going to the right (true)
        /// or left (false).
        /// </summary>
        public Dash(ICharacterController character, float dashSpeed, float dashTime)
        {
            timePassed = 0;
            this.dashSpeed = dashSpeed;
            this.character = character;
        }
        public void CancelMove()
        {

        }
        public void ContinueMove()
        {
            timePassed += Time.deltaTime;
        }
        public AnimationType GetAnimationState() => AnimationType.DASH;
        public bool IsMoveComplete() => timePassed > dashTime;
        public void StartMove()
        {
            timePassed = 0;
            switch (character.LeftOrRight)
            {
                case Facing.right:
                    character.SetSpeed(new Vector2(dashSpeed, 0));
                    break;
                case Facing.left:
                    character.SetSpeed(new Vector2(-dashSpeed, 0));
                    break;
            }
        }
    }
}
