using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the animation states that can take place in the game.
/// </summary>
public enum AnimationType
{
    /*
     * The integer value (order) of these enums is critical for the animator to work.
     * If any new value is added, it must be added to the end of the list.
    */
    NONE, // 0, excluded from animator
    IDLE, // 1, animated
    RUN, // 2, animated
    JUMP_RISING, // 3, animated
    JUMP_FALLING, // 4, animated
    DASH, // 5, animated
    KNOCKBACK, // 6, animated
    WALL_SLIDE, // 7, half-animated*
    WALL_JUMP, // 8, half-animated*
    WIRE_SWING // 9, half-animated*

    // *temporary sprite sheet used
}
