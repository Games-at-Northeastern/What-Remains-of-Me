using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    /// Moves player horizontally according to acceleration
    /// </summary>
    public class HorizontalSpeed : IMove
    {
        private float maxRunSpeed;
        private float acceleration;
        private float deceleration;
        CharacterController2D character;
        float xInput;
        public void UpdateDirection(float xInput)
        {
            this.xInput = xInput;
        }

        public HorizontalSpeed(float maxRunSpeed, float acceleration, float deceleration, CharacterController2D character)
        {

            if (deceleration < 0)
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
            Vector2 initialSpeed = speed;
            if (Mathf.Abs(speed.x) > Mathf.Abs(maxRunSpeed * xInput))
            {
                //decelerating
                speed.x = Kinematics.VelocityTowards(speed.x,deceleration, xInput * maxRunSpeed, Time.fixedDeltaTime);
            }
            else
            {
                speed.x = Kinematics.VelocityTowards(speed.x, acceleration, xInput * maxRunSpeed, Time.fixedDeltaTime);
            }
            //Debug.Log("before speed: " + initialSpeed + "AfterSpeed: " + speed);
            character.Speed = speed;
        }
        public AnimationType GetAnimationState() => throw new System.NotImplementedException();
        public bool IsMoveComplete() => true;

    }
}
