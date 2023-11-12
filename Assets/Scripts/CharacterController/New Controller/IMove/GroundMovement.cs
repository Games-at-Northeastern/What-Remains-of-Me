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
        float xInput;
        public void UpdateDirection(float xInput)
        {
            this.xInput = xInput;
        }
        public GroundMovement(float maxRunSpeed, float acceleration, float deceleration, ICharacterController character)
        {

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
            if(Mathf.Abs(speed.x) > Mathf.Abs(maxRunSpeed * xInput))
            {
                //decelerating
                speed.x = Kinematics.VelocityTarget(speed.x, Mathf.Sign(xInput) * deceleration, xInput * maxRunSpeed, Time.fixedDeltaTime);
            }
            else
            {
                speed.x = Kinematics.VelocityTarget(speed.x, acceleration * Mathf.Sign(xInput), xInput * maxRunSpeed, Time.fixedDeltaTime);
            }
            character.Speed = speed;
        }
        public AnimationType GetAnimationState() => throw new System.NotImplementedException();
        public bool IsMoveComplete() => true;

    }
}
