using System.Collections;
using UnityEngine;


/// <summary>
///     Temporarily activates a Cinemachine virtual camera when the player enters this trigger.
///     Holds for a set duration, then hands control back to whichever vcam was active before.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CinemachineViewTrigger : MonoBehaviour
{
    [Header("Cameras")]
    [Tooltip("The virtual camera to activate when the player enters this trigger.")]
    [SerializeField] private Unity.Cinemachine.CinemachineVirtualCamera targetVcam;

    [Header("Priorities")]
    [Tooltip("Priority to set on the target vcam while active. Should be higher than your player follow vcam.")]
    [SerializeField] private int activePriority = 20;
    [Tooltip("Priority to set on the target vcam while inactive.")]
    [SerializeField] private int inactivePriority = 0;

    [Header("Timing")]
    [Tooltip("How long to hold on the target view before handing control back.")]
    [SerializeField] private float holdDuration = 3f;

    [Header("Behavior")]
    [Tooltip("If true, this trigger only fires once and then disables itself.")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;
    private Coroutine activeRoutine;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Start()
    {
        // Make sure it starts inactive
        if (targetVcam != null)
            targetVcam.Priority = inactivePriority;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce)
            return;
        if (!other.CompareTag("Player"))
            return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ViewRoutine());

        if (triggerOnce)
            hasTriggered = true;
    }

    private IEnumerator ViewRoutine()
    {
        if (targetVcam == null)
        {
            Debug.LogWarning($"{name}: Target vcam not assigned.", this);
            yield break;
        }

        targetVcam.Priority = activePriority;
        yield return new WaitForSeconds(holdDuration);
        targetVcam.Priority = inactivePriority;

        activeRoutine = null;
    }

    private void OnDrawGizmos()
    {
        if (targetVcam != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetVcam.transform.position);
            Gizmos.DrawWireSphere(targetVcam.transform.position, 0.5f);
        }
    }
}
