using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDataCollector : MonoBehaviour
{
    /// <summary>
    /// Variable used to count player dashes on this level.
    /// </summary>
    [Tooltip("Variable used to count player dashes on this level.")]
    [SerializeField] private DatabaseInt _dashCountVariable;

    /// <summary>
    /// Variable used to count player dialogue on this level.
    /// </summary>
    [Tooltip("Variable used to count player dialogue on this level.")]
    [SerializeField] private DatabaseInt _dialogueCountVariable;

    /// <summary>
    /// Variable used to count player jumps on this level.
    /// </summary>
    [Tooltip("Variable used to count player jumps on this level.")]
    [SerializeField] private DatabaseInt _jumpCountVariable;

    /// <summary>
    /// Variable used to count player controller throws on this level.
    /// </summary>
    [Tooltip("Variable used to count player controller throws on this level.")]
    [SerializeField] private DatabaseInt _throwControllerCountVariable;

    /// <summary>
    /// Variable used to count player throws on this level.
    /// </summary>
    [Tooltip("Variable used to count player throws on this level.")]
    [SerializeField] private DatabaseInt _throwCountVariable;

    /// <summary>
    /// Variable used to count player mouse throws on this level.
    /// </summary>
    [Tooltip("Variable used to count player mouse throws on this level.")]
    [SerializeField] private DatabaseInt _throwMouseCountVariable;

    /// <summary>
    /// Method called when the player presses the controller throw input.
    /// </summary>
    /// <param name="context">Context passed to this argument by the
    /// InputActions asset.</param>
    public void OnControllerThrowInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _throwControllerCountVariable.Increment();
        }
    }

    /// <summary>
    /// Method called when the player presses the dash input.
    /// </summary>
    /// <param name="context">Context passed to this argument by the
    /// InputActions asset.</param>
    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _dashCountVariable.Increment();
        }
    }

    /// <summary>
    /// Method called when the player presses the dialogue input.
    /// </summary>
    /// <param name="context">Context passed to this argument by the
    /// InputActions asset.</param>
    public void OnDialogueInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _dialogueCountVariable.Increment();
        }
    }

    /// <summary>
    /// Method called when the player presses the jump input.
    /// </summary>
    /// <param name="context">Context passed to this argument by the
    /// InputActions asset.</param>
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _jumpCountVariable.Increment();
        }
    }

    /// <summary>
    /// Method called when the player presses the mouse throw input.
    /// </summary>
    /// <param name="context">Context passed to this argument by the
    /// InputActions asset.</param>
    public void OnMouseThrowInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _throwMouseCountVariable.Increment();
        }
    }

    /// <summary>
    /// Method called when the player presses the throw input.
    /// </summary>
    /// <param name="context">Context passed to this argument by the
    /// InputActions asset.</param>
    public void OnThrowInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _throwCountVariable.Increment();
        }
    }
}
