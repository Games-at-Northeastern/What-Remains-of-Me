using System;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    ///     Represents a move in which the player is jumping into the air.
    /// </summary>
    public class Jump : IMove
    {
        private readonly float initialVelocity; //The velocity at the start of the jump
        private float risingGravity; //The external force of gravity on the player
        private JumpType jumpType; //Determines how the initial velocity is added to the player (add vs. set)
        private CharacterController2D character; //The character that is jumping
        private readonly float timeToReachApex; //The time until the player reaches the peak of the jump
        private float timePassed; //The time that has passed since the player started jumping
        private bool jumpCanceled; //Has the jump been cancelled?

        /// <summary>
        /// Constructs a new jump, calculating the initial velocity based off of gravity + height,
        /// and timeToReachApex using that initial velocity. 
        /// </summary>
        public Jump(float risingGravity, float jumpHeight, JumpType jumpType, CharacterController2D character)
        {
            this.risingGravity = risingGravity;
            this.initialVelocity = Kinematics.InitialVelocity(0, risingGravity, jumpHeight);
            this.character = character;
            this.jumpType = jumpType;
            this.timeToReachApex = -initialVelocity / risingGravity;
        }

        /// <summary>
        /// Cancels this jump. 
        /// </summary>
        public void CancelMove() =>
            // empty for now but i think it can be useful in regards to jumpBuffers
            // though i may want that in the character controller itself
            jumpCanceled = true;

        /// <summary>
        /// Returns the animation state for this action. 
        /// </summary>
        public AnimationType GetAnimationState() => AnimationType.JUMP_RISING;

        /// <summary>
        /// is true after character hits apex of the jump
        /// </summary>
        /// <returns></returns>
        public bool IsMoveComplete() => timePassed >= timeToReachApex || jumpCanceled;

        /// <summary>
        /// Changes the character y velocity to what it should be at the beginning of this jump
        /// </summary>
        public void StartMove()
        {
            timePassed = 0;
            jumpCanceled = false;
            switch (jumpType)
            {
                case JumpType.addSpeed:
                    character.InternalVelocity = new Vector2(character.InternalVelocity.x, character.InternalVelocity.y + initialVelocity);
                    break;
                case JumpType.setSpeed:
                    character.InternalVelocity = new Vector2(character.InternalVelocity.x, initialVelocity);
                    break;
            }
        }

        /// <summary>
        /// decreases gravity while the player holds down space;
        /// </summary>
        public void ContinueMove()
        {
            timePassed += Time.fixedDeltaTime;
            character.InternalVelocity = new Vector2(character.InternalVelocity.x, character.InternalVelocity.y - (risingGravity * Time.fixedDeltaTime));
        }
    }
    
    /// <summary>
    /// There are 2 jump types
    /// AddSpeed: will add some Velocity Value v to your current y Speed
    /// SetSpeed: will set your y speed to v
    /// </summary>
    public enum JumpType
    {
        addSpeed,
        setSpeed
    }
}
