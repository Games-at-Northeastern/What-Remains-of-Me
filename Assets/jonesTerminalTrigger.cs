using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class jonesTerminalTrigger : MonoBehaviour
{
    [Header("Custom Event")]
    public UnityEvent myEvents;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (myEvents == null)
        {
            print("myEventTriggerOnEnter was triggered but myEvents was null");
        }
        else
        {
            print("myEventTriggerOnEnter Activated. Triggering" + myEvents);
            myEvents.Invoke();
        }
    }
}
