using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    /// Represents a move in which the player dashes, on air or on ground,
    /// at a quick speed.
    /// </summary>
    public class Dash : IMove
    {
        public void CancelMove() => throw new System.NotImplementedException();
        public void ContinueMove() => throw new System.NotImplementedException();
        public AnimationType GetAnimationState() => throw new System.NotImplementedException();
        public bool IsMoveComplete() => throw new System.NotImplementedException();
        public void StartMove() => throw new System.NotImplementedException();
    }
}
