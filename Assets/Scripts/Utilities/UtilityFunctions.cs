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
    /// <typeparam name="T">Un-parameterized type of given item</typeparam>
    /// <param name="item">The given item to check if null</param>
    /// <param name="msg">What msg to print if item is null</param>
    /// <returns>The item given to it regardless if it is null or not</returns>
    public static T RequireNonNull<T>(T item, string msg)
    {
        if (item == null)
        {
            Debug.LogError(msg);
        }

        return item;
    }


    /// <summary>
    /// Given a child transform, checks it and its parents to see if they are tagged with the
    /// given tag. Returns true if the tag is found, false otherwise.
    /// </summary>
    /// <param name="root">The transform to start searching from. Included in the tag checking.</param>
    /// <param name="tag">The tag to search for.</param>
    /// <param name="found">The item that the tag was found on.</param>
    /// <param name="layersToCheck">How many "generations" up the parent hierarchy we check. 1 = only check the root.</param>
    /// <returns>True if the tag was found, false if not.</returns>
    public static bool CompareTagOfHierarchy(Transform root, string tag, out Transform found, int layersToCheck = 2)
    {
        var item = root;

        for (int layer = 0; layer < layersToCheck; layer += 1)
        {
            if (item == null)
            {
                break;
            }

            if (item.CompareTag(tag))
            {
                found = item;
                return true;
            }

            item = item.parent;
        }

        found = null;
        return false;
    }

    /// <summary>
    /// Returns true if a value is within the (inclusive) min and (exclusive) max.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool ValueInRange(float value, float min, float max) => value >= min && value < max;
}
