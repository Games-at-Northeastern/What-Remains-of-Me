using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A button that can be used to activate an AObjectToActivate
/// </summary>
public class Button : MonoBehaviour
{
    public AObjectToActivate OTA; // The object to activate
    public bool CheckTag; // Check the tag of the collision?
    public string TagOfOther; // Tag of the collision to check

    /// <summary>
    /// Activate the object if something collides with this button
    /// </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!CheckTag || other.gameObject.tag == TagOfOther)
        {
            OTA.Activate();
        }
    }
}
