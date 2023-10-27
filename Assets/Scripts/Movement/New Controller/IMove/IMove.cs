
namespace CharacterController
{
    /// <summary>
    /// Represents a move that a character can do
    /// Note: when implementing this interface you can pass anything you want the move to know into the constructor
    /// if moves need to be updated with a new inputs you can include a method to update the info stored in a move
    /// ie if you want to update the horizontal input for a run without creating a new object. 
    /// </summary>
    public interface IMove
    {
        /// <summary>
        /// Method meant to be called when a move has just begun to be used
        /// used for if you only want something to be done once at the beginning of a move
        /// </summary>
        void StartMove();
        /// <summary>
        /// Method meant to be called once per fixedUpdate for anything that needs to be done
        /// as the move is being executed
        /// </summary>
        void ContinueMove();
        /// <summary>
        /// Method meant to be called once the move is complete or
        /// the controller wants to end the move early in case there is any cleanup or extra
        /// functionality that needs to be done on move end the character controller would likely
        /// call Move1.CancelMove() and Move2.StartMove() on the same frame. 
        /// </summary>
        void CancelMove();
        /// <summary>
        /// Has this move completed everything it has wanted to do.
        /// Controller does not need to account for if a move is finished
        /// but it's here if controller want to lock out other inputs until a move is complete
        /// </summary>
        /// <returns></returns>
        bool IsMoveComplete();
        /// <summary>
        /// returns the animation state for this kind of move
        /// </summary>
        /// <returns></returns>
        AnimationType GetAnimationState();
    }
}
