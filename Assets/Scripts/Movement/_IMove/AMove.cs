using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds information and functionality useful to all moves. Certain fields
/// are static so as to make the transfer of information between moves easy,
/// and because only one player should be moving at any given time.
/// </summary>
public abstract class AMove : IMove
{
    private static bool flipped = false;
    protected static bool dashIsReset = false; // Is the player in a state where a dash would be okay? Needs to be checked before transitioning into a dive.
    protected static MovementSettings MS { get; private set; } // For constants related to movement
    protected static MovementInfo MI { get; private set; } // For useful class instances related to movement
    protected static ControlSchemes CS { get; private set; } // For input reading
    protected static WireThrower WT { get; private set; } // Gives information about the wire
    protected static PlayerHealth PH { get; private set; } // Gives information about health/damage

    /// <summary>
    /// Reset variables for scene load.
    /// </summary>
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        dashIsReset = false;
        flipped = false;
        Debug.Log("SCENE LOADED");
    }

    /// <summary>
    /// Initializes all the static fields of this class so that they can be accessed
    /// by base classes in the future.
    /// To be called once, and only once, at the very beginning of the player's movement
    /// every time a scene loads.
    /// </summary>
    protected void Initialize(MovementInfo mi, MovementSettings ms, ControlSchemes cs, WireThrower wt, PlayerHealth ph)
    {
        MS = UtilityFunctions.RequireNonNull(ms, "MovementSettings can't be null in AMove.");
        MI = UtilityFunctions.RequireNonNull(mi, "MovementInfo can't be null in AMove.");
        CS = UtilityFunctions.RequireNonNull(cs, "ControlSchemes can't be null in AMove.");
        WT = UtilityFunctions.RequireNonNull(wt, "WireThrower can't be null in AMove.");
        PH = UtilityFunctions.RequireNonNull(ph, "PlayerHealth can't be null in AMove.");
        SceneManager.sceneLoaded += OnSceneLoad;
        Debug.Log("INITIALIZE");
    }

    // All the below methods are implementations of IMove/IMoveImmutable methods

    public abstract void AdvanceTime();

    public abstract IMove GetNextMove();

    public abstract float XSpeed();

    public abstract float YSpeed();

    public abstract AnimationType GetAnimationState();

    public virtual bool DisconnectByJumpOkay()
    {
        return false;
    }

    public bool Flipped()
    {
        if (CS == null) // If AMove not totally initialized yet
        {
            return flipped;
        }
        float moveInput = CS.Player.Move.ReadValue<float>();
        bool flippedNow = flipped ? (moveInput <= 0) : (moveInput < 0);
        flipped = flippedNow;
        return flippedNow;
    }

    /// <summary>
    /// Get the length of the wire connecting to an outlet. You should be
    /// connected to an outlet if you're calling this function.
    /// </summary>
    protected float GetCurrentWireLength()
    {
        Vector2 origPos = MI.transform.position;
        Vector2 connectedOutletPos = WT.ConnectedOutlet.transform.position;
        return Vector2.Distance(origPos, connectedOutletPos);
    }
}
