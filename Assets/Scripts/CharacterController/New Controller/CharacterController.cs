namespace CharacterController
{
    using UnityEngine;

    /// A controller for a character. that uses a rigidBody2D for collision detection.
    /// it's velocity can be changed to move the character upon each fixed frame
    public interface CharacterController2D
    {

        /// the current speed of the character at the end of the frame the attached rigidbody2D velocity is mutated to this value to move the character.
        public Vector2 InternalVelocity { get; set; }
        /// the current velocity of the character according to the attached rigidBody2d
        public Vector2 Velocity { get; }
        public Vector3 position { get; }
        /// checks if the player character is grounded
        public bool Grounded { get; }
        /// returns the forward vector of the player
        public Vector2 Direction { get; }
        /// returns whether the character is facing leftOrRight
        public Facing LeftOrRight { get; }
        /// stops the character from receiving any inputs and/or doing moves. until unfrozen may 
        // still be subject to some physics interactions
        public void LockInputs();
        /// Enables the character to receive inputs again and do moves
        public void UnlockInputs();


        // these will check player collisions
        public bool TouchingLeftWall();
        public bool TouchingRightWall();
        public bool TouchingCeiling();
        public bool TouchingGround();

        public AnimationType GetAnimationState();
    }

    // this checks what direction the player character is facing
    public enum Facing
    {
        left,
        right
    }

}
