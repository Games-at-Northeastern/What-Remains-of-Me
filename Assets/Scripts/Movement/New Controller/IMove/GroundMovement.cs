using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    public class GroundMovement : IMove
    {
        private float maxRunSpeed;
        private float acceleration;
        private float deceleration;
        ICharacterController character;
        Vector2 direction;
        public void UpdateDirection(Vector2 direction)
        {
            this.direction = direction;
            if(this.direction.magnitude > 1)
            {
                direction.Normalize();
            }
        }
        public GroundMovement(Vector2 startDirection, float maxRunSpeed, float acceleration, float deceleration, ICharacterController character)
        {
            direction = startDirection;
            if(direction.magnitude > 1)
            {
                direction.Normalize();
            }
            if(deceleration > 0)
            {
                Debug.LogWarning("Deceleration should be a negative value");
            }
            if(acceleration < 0)
            {
                Debug.LogWarning("Acceleration should be a positive valuie");
            }
            this.maxRunSpeed = maxRunSpeed;
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            this.character = character;
        }
        public void StartMove() { }
        public void CancelMove() { }
        public void ContinueMove()
        {
            Vector2 speed = character.Speed;
            if(Mathf.Abs(speed.x) > Mathf.Abs(maxRunSpeed * direction.x))
            {
                //decelerating
                speed.x = Kinematics.VelocityTarget(speed.x, Mathf.Sign(direction.x) * deceleration, direction.x * maxRunSpeed, Time.fixedDeltaTime);
            }
            else
            {
                speed.x = Kinematics.VelocityTarget(speed.x, acceleration * Mathf.Sign(direction.x), direction.x * maxRunSpeed, Time.fixedDeltaTime);
            }
            character.Speed = speed;
        }
        public AnimationType GetAnimationState() => throw new System.NotImplementedException();
        public bool IsMoveComplete() => true;

    }
}
