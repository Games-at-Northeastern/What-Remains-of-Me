using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of the move running in MoveExecuter as it changes.
/// </summary>
[RequireComponent(typeof(MovementExecuter))]
public class MovementDebugger : MonoBehaviour
{
    MovementExecuter me;
    IMoveImmutable storedMove = null;

    private void Awake()
    {
        me = GetComponent<MovementExecuter>();
    }

    // Check if the move this frame is any different, and if so, prints the change.
    private void Update()
    {
        IMoveImmutable moveThisFrame = me.GetCurrentMove();
        if (moveThisFrame != storedMove)
        {
            print(storedMove + " => " + moveThisFrame);
            storedMove = moveThisFrame;
        }
    }
}
