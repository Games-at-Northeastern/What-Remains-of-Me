using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    public class AirMovement : IMove
    {
        Vector2 direction;
        float maxAirSpeed, terminalVelocity, airAcceleration, fallGravity;
        ICharacterController character;
        public AirMovement(Vector2 direction, float maxAirSpeed, float terminalVelocity, float airAcceleration, float fallGravity, ICharacterController character)
        {
            if (maxAirSpeed < 0)
            {
                Debug.LogWarning("MaxAirSpeed should be positive");
            }
            if (airAcceleration < 0)
            {
                Debug.LogWarning("airAcceleration should be positive");
            }
            if (Mathf.Sign(terminalVelocity) == Mathf.Sign(fallGravity))
            {
                Debug.LogWarning("you likely want terminal Velocity and fall gravity to share the same sign");
            }
            this.direction = direction;
            this.character = character;
            this.maxAirSpeed = maxAirSpeed;
            this.terminalVelocity = terminalVelocity;
            this.airAcceleration = airAcceleration;
            this.fallGravity = fallGravity;
        }
        public void UpdateDirection(Vector2 direction)
        {
            this.direction = direction;
        }
        public void CancelMove() { }
        public void ContinueMove()
        {
            Vector2 speed = character.Speed;
            speed.x = Kinematics.VelocityTarget(speed.x, maxAirSpeed * direction.x, airAcceleration, Time.fixedDeltaTime);
            speed.y = Kinematics.VelocityTarget(speed.y, terminalVelocity, fallGravity, Time.fixedDeltaTime);
            character.SetSpeed(speed);
        }
        public AnimationType GetAnimationState() => AnimationType.JUMP_FALLING;
        public bool IsMoveComplete() => character.Grounded;
        public void StartMove()
        {

        }
    }
}
