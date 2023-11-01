using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    /// <summary>
    /// Represents a move where the player is swinging from side to side whilst
    /// connected to a wire.
    /// </summary>
    public class WireSwing : IMove
    {
        public void CancelMove() => throw new System.NotImplementedException();
        public void ContinueMove() => throw new System.NotImplementedException();
        public AnimationType GetAnimationState() => throw new System.NotImplementedException();
        public bool IsMoveComplete() => throw new System.NotImplementedException();
        public void StartMove() => throw new System.NotImplementedException();
    }
}
