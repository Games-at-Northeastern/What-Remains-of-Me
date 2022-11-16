using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A button that can be used to activate an AObjectToActivate. It needs to be kept pressing to activate
/// </summary>
public class HoldButton : Button
{
    /// <summary>
    /// Deactivate when no longer pressed
    /// </summary>
    void OnCollisionExit2D(Collision2D other)
    {
        if (!CheckTag || other.gameObject.tag == TagOfOther)
        {
            OTA.Deactivate();
        }
    }
}
