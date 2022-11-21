using UnityEngine;

/// <summary>
/// Allows designers to configure responses to OnTriggerEnter, OnTriggerStay,
/// and OnTriggerExit with a specific tag all in the inspector.
/// </summary>
public class TriggerDataRelay2D : MonoBehaviour
{
    /// <summary>
    /// Collection of responses to trigger detection.
    /// </summary>
    [SerializeField] private TriggerResponse[] _triggerResponses;

    #region MonoBehaviour Methods
    private void OnTriggerEnter2D(Collider2D other) 
    {
        foreach (TriggerResponse triggerResponse in _triggerResponses)
        {
            if (other.transform.CompareTag(triggerResponse.Tag))
            {
                triggerResponse.OnTriggerEnterResponse?.Invoke();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        foreach (TriggerResponse triggerResponse in _triggerResponses)
        {
            if (other.transform.CompareTag(triggerResponse.Tag))
            {
                triggerResponse.OnTriggerStayResponse?.Invoke();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        foreach (TriggerResponse triggerResponse in _triggerResponses)
        {
            if (other.transform.CompareTag(triggerResponse.Tag))
            {
                triggerResponse.OnTriggerExitResponse?.Invoke();
            }
        }
    }
    #endregion
}
