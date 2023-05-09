using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an event of interest to many other classes. The Raise() function
/// is called when the event is triggered, signaling all listeners to respond
/// to a particular game event.
/// </summary>
[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    /// <summary>
    /// List of listeners interested in this event. When the event is raised,
    /// each listener's "OnEventRaised()" method will be called. Click any of
    /// the list's entries to ping the GameObject the listener is attached to.
    /// </summary>
    [Tooltip("List of listeners interested in this event. When the event is " + 
    "raised, each listener's 'OnEventRaised()' method will be called. Click " + 
    "any of the list's entries to ping the GameObject the listener is " + 
    "attached to.")]
    [SerializeField]
    private List<GameEventListener> _listeners =
        new List<GameEventListener>();
        
    /// <summary>
    /// Calls the OnEventRaised method of all registered listeners in reverse
    /// order. The reverse order allows listeners to be removed from the
    /// listener list as a result of a raised event.
    /// </summary>
    public void Raise()
    {
        if (_listeners.Count > 0)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }
    }

    /// <summary>
    /// Adds a listener to the list of listeners. Each listener in the list
    /// will have its OnEventRaised method called when this GameEvent is raised.
    /// </summary>
    /// <param name="listener">The listener to add to this GameEvent's list of
    /// listeners.</param>
    public void RegisterListener(GameEventListener listener)
    {
        _listeners.Add(listener);
    }

    /// <summary>
    /// Removes a listener to the list of listeners, if the passed listener
    /// exists in the list.
    /// </summary>
    /// <param name="listener">The listener to remove from this GameEvent's list
    /// of listeners.</param>
    public void UnregisterListener(GameEventListener listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }
}