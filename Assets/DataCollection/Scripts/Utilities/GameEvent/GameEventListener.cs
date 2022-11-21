using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class to be attached to GameObjects, allowing them to respond to specific
/// GameEvent raise calls. Unity Event Responses can be configured in the 
/// inspector, but the scope of all references should be within a single 
/// GameObject.
/// </summary>
[System.Serializable]
public class GameEventListener : MonoBehaviour
{
    /// <summary>
    /// GameEvent that this listener will listen to.
    /// </summary>
    [Tooltip("GameEvent that this listener will listen to.")]
    public GameEvent Event;

    /// <summary>
    /// Responses to trigger when the event is raised.
    /// </summary>
    [Tooltip("Responses to trigger when the event is raised.")]
    public UnityEvent Response;

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        Event.RegisterListener(this);
    }
    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }
    #endregion

    /// <summary>
    /// Responds the the assigned GameEvent's raise call by invoking a Unity
    /// Event.
    /// </summary>
    public void OnEventRaised()
    {
        Response.Invoke();
    }
}
