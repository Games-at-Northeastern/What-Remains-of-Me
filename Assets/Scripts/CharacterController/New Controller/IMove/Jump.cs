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
        private ICharacterController character;
        private readonly float timeToReachApex;
        private float timePassed;
        public Jump(float risingGravity, float jumpHeight, JumpType jumpType, ICharacterController character)
        {
            this.risingGravity = risingGravity;
            this.initialVelocity = Kinematics.InitialVelocity(0, risingGravity, jumpHeight);
            this.character = character;
            this.jumpType = jumpType;
            this.timeToReachApex = -initialVelocity / risingGravity;
            Debug.Log("IV" + initialVelocity + Kinematics.InitialVelocity(0, risingGravity, jumpHeight));
        }
        public void CancelMove()
        {
            // empty for now but i think it can be useful in regards to jumpBuffers
            // though i may want that in the character controller itself
        }

        public AnimationType GetAnimationState() => AnimationType.JUMP_RISING;
        /// <summary>
        /// is true after character hits apex of the jump
        /// </summary>
        /// <returns></returns>
        public bool IsMoveComplete() => timePassed >= timeToReachApex;
        /// <summary>
        /// Changes the character y velocity to what it should be at the beginning of this jump
        /// </summary>

        public void StartMove()
        {
            timePassed = 0;
            switch (jumpType)
            {
                case JumpType.addSpeed:
                    character.Speed.Set(character.Speed.x, character.Speed.y + initialVelocity);
                    break;
                case JumpType.setSpeed:
                    Debug.Log("IN switch");
                    character.Speed = new Vector2(character.Speed.x, initialVelocity);
                    break;
            }
            Debug.Log("JumpSpeed" + character.Speed.y);

        }
        /// <summary>
        /// decreases the vertical velocity by gravity;
        /// </summary>
        public void ContinueMove()
        {
            timePassed += Time.fixedDeltaTime;
            character.Speed.Set(character.Speed.x, character.Speed.y - risingGravity * Time.fixedDeltaTime);
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
