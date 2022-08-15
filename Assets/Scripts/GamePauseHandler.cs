using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// There are many different game states that could lead to the game
/// being paused (i.e. being on the pause screen, being in a dialogue sequence,
/// etc.) This script keeps track of all those variables and uses them to
/// determine whether the game should be paused or not.
/// </summary>
public class GamePauseHandler : MonoBehaviour
{
    static bool gamePausedForDialogue;

    // Reset static variables so that the game never starts out paused
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetDomain()
    {
        gamePausedForDialogue = false;
    }

    void Awake()
    {
        UpdateTimeScale();
    }

    /// <summary>
    /// Document whether or not the game is paused for dialogue.
    /// </summary>
    public static void setPausedForDialogue(bool state)
    {
        gamePausedForDialogue = state;
        UpdateTimeScale();
    }

    /// <summary>
    /// According to what is paused in the game state, determines the time scale.
    /// </summary>
    private static void UpdateTimeScale()
    {
        Time.timeScale = gamePausedForDialogue ? 0 : 1;
    }
}
