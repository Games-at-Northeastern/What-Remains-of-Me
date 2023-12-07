using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    /// Simulates gravity with a terminal velocity
    /// </summary>
    public class VerticalFallSpeed : IMove
    {
        float terminalVelocity, fallGravity;
        ICharacterController character;
        public VerticalFallSpeed(float terminalVelocity, float fallGravity, ICharacterController character)
        {
            if (Mathf.Sign(terminalVelocity) == Mathf.Sign(fallGravity))
            {
                Debug.LogWarning("you likely want terminal Velocity and fall gravity to share the same sign");
            }
            this.character = character;
            this.terminalVelocity = terminalVelocity;
            this.fallGravity = fallGravity;
        }
        public void CancelMove() { }
        public void ContinueMove()
        {
            Vector2 speed = character.Speed;
            speed.y = Kinematics.VelocityTarget(speed.y, terminalVelocity, fallGravity, Time.fixedDeltaTime);
            character.Speed = speed;
        }
        public AnimationType GetAnimationState() => AnimationType.JUMP_FALLING;
        public bool IsMoveComplete() => character.Grounded;
        public void StartMove()
        {

        }
    }
}
