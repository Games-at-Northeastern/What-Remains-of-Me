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

            if (deceleration > 0)
            {
                Debug.LogWarning("Deceleration should be a positive value");
            }
            if (acceleration < 0)
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
            Vector2 b4 = speed;
            if (Mathf.Abs(speed.x) > Mathf.Abs(maxRunSpeed * xInput))
            {
                //decelerating
                speed.x = Kinematics.VelocityTowards(speed.x,deceleration, xInput * maxRunSpeed, Time.fixedDeltaTime);
            }
            else
            {
                speed.x = Kinematics.VelocityTowards(speed.x, acceleration, xInput * maxRunSpeed, Time.fixedDeltaTime);
            }
            Debug.Log("before speed: " + b4 + "AfterSpeed: " + speed);
            character.Speed = speed;
        }
        public AnimationType GetAnimationState() => throw new System.NotImplementedException();
        public bool IsMoveComplete() => true;

    }
}
