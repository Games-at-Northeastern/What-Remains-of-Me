using System;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    ///     Represents a move in which the player is jumping into the air.
    /// </summary>
    public class Jump : IMove
    {
        private readonly float initialVelocity;
        private float risingGravity;
        private JumpType jumpType;
        private CharacterController2D character;
        private readonly float timeToReachApex;
        private float timePassed;
        private bool jumpCanceled;
        public Jump(float risingGravity, float jumpHeight, JumpType jumpType, CharacterController2D character)
        {
            this.risingGravity = risingGravity;
            this.initialVelocity = Kinematics.InitialVelocity(0, risingGravity, jumpHeight);
            this.character = character;
            this.jumpType = jumpType;
            this.timeToReachApex = -initialVelocity / risingGravity;
        }
        public void CancelMove() =>
            // empty for now but i think it can be useful in regards to jumpBuffers
            // though i may want that in the character controller itself
            jumpCanceled = true;

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
                    character.Speed = new Vector2(character.Speed.x, character.Speed.y + initialVelocity);
                    break;
                case JumpType.setSpeed:
                    character.Speed = new Vector2(character.Speed.x, initialVelocity);
                    break;
            }
        }
        /// <summary>
        /// decreases gravity while the player holds down space;
        /// </summary>
        public void ContinueMove()
        {
            timePassed += Time.fixedDeltaTime;
            character.Speed = new Vector2(character.Speed.x, character.Speed.y - (risingGravity * Time.fixedDeltaTime));
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
