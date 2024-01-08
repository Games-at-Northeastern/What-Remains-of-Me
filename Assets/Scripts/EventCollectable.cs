using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Fires an event if this object is touched by the player. Destroys this object once
/// collected.
/// </summary>
public class EventCollectable : MonoBehaviour
{
    public UnityEvent onCollected;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            onCollected.Invoke();
            Destroy(gameObject);
        }
    }
}
