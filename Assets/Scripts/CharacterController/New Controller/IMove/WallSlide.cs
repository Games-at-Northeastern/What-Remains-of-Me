using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    /// Represents a move in which the player is sliding down a wall, preparing
    /// for a wall jump.
    /// </summary>
    public class WallSlide : IMove
    {
        private float slideGravity;
        private ICharacterController character;
        private float maxWallSlideSpeed;
        public WallSlide(float slideGravity, float maxWallSlideSpeed, ICharacterController character)
        {
            this.character = character;
            this.slideGravity = slideGravity;
            this.maxWallSlideSpeed = Mathf.Abs(maxWallSlideSpeed);

        }
        public void CancelMove()
        {
            //Currently do nothing
        }
        public void ContinueMove()
        {
            var speed = character.Speed;
            speed.y = Kinematics.VelocityTarget(character.Speed.y, slideGravity, Mathf.Sign(slideGravity) * maxWallSlideSpeed, Time.fixedDeltaTime);
            character.Speed = (speed);
        }
        public AnimationType GetAnimationState() => AnimationType.WALL_SLIDE;
        /// <summary>
        /// move isn't complete while the character is not grounded
        /// </summary>
        /// <returns></returns>
        //devnote: may want to in future take into if player is holding into the wall. but as of rn seems more of a job for the character controller 
        public bool IsMoveComplete() => !character.Grounded;
        public void StartMove()
        {
            //currently when you start sliding you either decelerate straight to max wall slide speed or keep your y speed
            //would maybe want to change this later
            character.Speed.Set(0, Mathf.Clamp(character.Speed.y, -maxWallSlideSpeed, maxWallSlideSpeed));
        }
    }
}
