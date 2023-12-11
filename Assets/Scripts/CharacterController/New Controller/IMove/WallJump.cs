
using UnityEngine;
namespace CharacterController
{


    /// <summary>
    /// Represents a move in which the player is jumping away from a wall.
    /// </summary>
    public class WallJump : IMove
    {
        private float RisingGravity;
        private Vector2 distance;
        private float lockOutInputTime;
        private CharacterController2D character;
        private float timePassed;
        /// <summary>
        /// Defines A Characters Wall Jump Move
        /// </summary>
        /// <param name="character"> the CharacterController to have enact the move</param>
        /// <param name="RisingGravity"> Gravity applied during the startup of the jump</param>
        /// <param name="distance"> how high wall Jump takes the character and  and how far in the x direction the character traveled when at the apex of the jump</param>
        /// <param name="lockOutInputTime"> How long Input should be taked away from the player to avoiding moving back into the wall immediately</param>
        public WallJump(CharacterController2D character, float RisingGravity, Vector2 distance, float lockOutInputTime)
        {
            this.character = character;
            this.RisingGravity = RisingGravity;
            this.distance = distance;
            this.lockOutInputTime = lockOutInputTime;
        }
        public void StartMove()
        {
            timePassed = 0;
            var initY = Kinematics.InitialVelocity(0, RisingGravity, distance.y);
            if (character.LeftOrRight == Facing.left)
            {
                character.Speed = (new Vector2(distance.x / (initY / RisingGravity), initY));
            }
            else
            {
                character.Speed = (new Vector2(-distance.x / (initY / RisingGravity), initY));
            }
        }
        public void ContinueMove()
        {
            character.Speed = (new Vector2(character.Speed.x, character.Speed.y - RisingGravity));
            timePassed += Time.deltaTime;
        }
        public void CancelMove()
        {

        }

        public AnimationType GetAnimationState() => AnimationType.WALL_JUMP;
        public bool IsMoveComplete() => timePassed >= lockOutInputTime;

    }
}
