
using UnityEngine;
namespace CharacterController
{


    /// <summary>
    /// Represents a move in which the player is jumping away from a wall.
    /// </summary>
    public class WallJump : IMove
    {
        public void CancelMove() => throw new System.NotImplementedException();
        public void ContinueMove() => throw new System.NotImplementedException();
        public AnimationType GetAnimationState() => throw new System.NotImplementedException();
        public bool IsMoveComplete() => throw new System.NotImplementedException();
        public void StartMove() => throw new System.NotImplementedException();
    }
}
