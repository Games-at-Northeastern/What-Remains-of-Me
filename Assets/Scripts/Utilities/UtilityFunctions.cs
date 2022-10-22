using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides useful general utilities for other classes. Currently provides a
/// null checker for a given item.
/// </summary>
public class UtilityFunctions
{
    /// <summary>
    /// Checks if a given item is null. If it is then debug to the console
    /// with the given message.
    /// </summary>
    /// <typeparam name="T">Unparameterized type of given item</typeparam>
    /// <param name="item">The given item to check if null</param>
    /// <param name="msg">What msg to print if item is null</param>
    /// <returns>The item given to it regardless if it is null or not</returns>
    public static T RequireNonNull<T>(T item, string msg)
    {
       if (item == null) {
           Debug.LogError(msg);
       }
       return item;
    }
}
