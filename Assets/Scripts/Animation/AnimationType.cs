using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
///  * This enum represents the animation states of the player character. 
///  This is done through correlating integers to these animation states.
///  Animation to Integer Relationship:
///  None = 0
///  Idle = 1
///  Run = 2
///  Jump up = 3
///  Falling = 4
///  Dash = 5
///  Knockback = 6
///  Wall slide = 7
///  Wall jump = 8
///  Wire swing = 9
/// </summary>
public enum AnimationType
{
    /// <summary>
    /// The integer value (order) of these enums is critical for the animator to work.
    /// If any new value is added, it must be added to the end of the list.
    /// </summary>
   
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
