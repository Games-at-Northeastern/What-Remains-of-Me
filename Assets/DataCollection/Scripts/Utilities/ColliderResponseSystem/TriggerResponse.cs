using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerResponse
{
    /// <summary>
    /// Which tag should this trigger response detect?
    /// </summary>
    [Tooltip("Which tag should this trigger response detect?")]
    public string Tag;

    /// <summary>
    /// What should respond to the OnTriggerEnter event?
    /// </summary>
    [Tooltip("What should respond to the OnTriggerEnter event?")]
    public UnityEvent OnTriggerEnterResponse;

    /// <summary>
    /// What should respond to the OnTriggerStay event?
    /// </summary>
    [Tooltip("What should respond to the OnTriggerStay event?")]
    public UnityEvent OnTriggerStayResponse;

    /// <summary>
    /// What should respond to the OnTriggerExit event?
    /// </summary>
    [Tooltip("What should respond to the OnTriggerExit event?")]
    public UnityEvent OnTriggerExitResponse;
}
