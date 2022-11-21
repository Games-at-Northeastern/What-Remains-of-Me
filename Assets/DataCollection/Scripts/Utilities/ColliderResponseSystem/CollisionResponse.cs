using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows selection of a tag to detect in the inspector, and allows for 
/// responses to each of the three MonoBehavior collision events to be
/// configured in the inspector.
/// </summary>
[System.Serializable]
public class CollisionResponse
{
    /// <summary>
    /// Which tag should this collision response detect?
    /// </summary>
    [Tooltip("Which tag should this collision response detect?")]
    public string Tag;

    /// <summary>
    /// What should respond to the OnCollisionEnter event?
    /// </summary>
    [Tooltip("What should respond to the OnCollisionEnter event?")]
    public UnityEvent OnCollisionEnterResponse;

    /// <summary>
    /// What should respond to the OnCollisionStay event?
    /// </summary>
    [Tooltip("What should respond to the OnCollisionStay event?")]
    public UnityEvent OnCollisionStayResponse;

    /// <summary>
    /// What should respond to the OnCollisionExit event?
    /// </summary>
    [Tooltip("What should respond to the OnCollisionExit event?")]
    public UnityEvent OnCollisionExitResponse;
}
