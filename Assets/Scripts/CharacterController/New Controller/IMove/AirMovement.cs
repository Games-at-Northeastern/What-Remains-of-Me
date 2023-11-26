using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    public class AirMovement : IMove
    {
        Vector2 direction;
        float maxAirSpeed, terminalVelocity, airAcceleration,airDeceleration, fallGravity, maxPlayerInputAirSpeed;
        ICharacterController character;
        public AirMovement(float maxPlayerInputAirSpeed, float maxAirSpeed, float terminalVelocity, float airAcceleration, float airDeceleration, float fallGravity, ICharacterController character)
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
            this.airAcceleration = airAcceleration;
            this.maxPlayerInputAirSpeed = maxPlayerInputAirSpeed;
            direction = Vector2.zero;
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
            if (Mathf.Abs(character.Speed.x) > maxPlayerInputAirSpeed)
            {
                speed.x = Kinematics.VelocityTowards(speed.x, airDeceleration, maxPlayerInputAirSpeed * Mathf.Sign(character.Velocity.x), Time.fixedDeltaTime);
            }
            else
            {
                speed.x = Kinematics.VelocityTarget(speed.x, maxAirSpeed * direction.x, airAcceleration, Time.fixedDeltaTime);
            }
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
