using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an object that can be activated by a button
/// </summary>
public abstract class AObjectToActivate : MonoBehaviour
{
    /// <summary>
    /// Does things when this object is activated
    /// </summary>
    public abstract void Activate();
    /// <summary>
    /// Does things when this object is deactivated
    /// </summary>
    public abstract void Deactivate();
}
