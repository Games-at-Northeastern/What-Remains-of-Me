using UnityEngine;

public class DetectingDoorTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    void Start()
    {
        hasTriggered = false;
    }

    [SerializeField]
    private ControllableDoor controlled;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("Player walked in");
            controlled.setMaxCharge();
            hasTriggered = true;
        }
    }
}
