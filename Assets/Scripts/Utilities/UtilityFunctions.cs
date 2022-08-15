using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides useful general utilities for other classes.
/// </summary>
public class UtilityFunctions
{
    /// <summary>
    /// Makes the given error message appear if the given item is null.
    /// Returns the item whether or not it is null.
    /// </summary>
    public static T RequireNonNull<T>(T item, string msg)
    {
        if (item == null)
        {
            Debug.LogError(msg);
        }
        return item;
    }
}
