namespace CharacterController
{
    using UnityEngine;

    /// <summary>
    /// A controller for a character. that uses a rigidBody2D for collision detection.
    /// it's velocity can be changed to move the character upon each fixed frame
    /// </summary>
    public interface CharacterController2D
    {

        /// <summary>
        /// the current speed of the character
        /// at the end of the frame the attached rigidbody2D velocity is mutated to this value to move the character.
        /// </summary>
        public Vector2 Speed { get; set; }
        /// <summary>
        /// the current velocity of the character according to the attached rigidBody2d
        /// </summary>
        public Vector2 Velocity { get; }
        public Vector3 position { get; }
        /// <summary>
        /// is the 
        /// </summary>
        public bool Grounded { get; }
        /// <summary>
        /// returns the forward vector of the player
        /// </summary>
        public Vector2 Direction { get; }
        /// <summary>
        /// returns whether the character is facing leftOrRight
        /// </summary>
        public Facing LeftOrRight { get; }
        /// <summary>
        /// stops the character from receiving any inputs and/or doing moves. until unfrozen
        /// may still be subject to some physics interactions
        /// </summary>
        public void LockInputs();
        /// <summary>
        /// Enables the character to receive inputs again
        /// and do moves
        /// </summary>
        public void UnlockInputs();

        public bool TouchingLeftWall();
        public bool TouchingRightWall();
        public bool TouchingCeiling();
        public bool TouchingGround();

        public AnimationType GetAnimationState();
    }
    public enum Facing
    {
        left,
        right
    }

}
