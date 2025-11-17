using UnityEngine;

public class KeyDoorTrigger : MonoBehaviour
{
    [SerializeField] private AutoOpenDoor autoOpenDoor;
    void Start()
    {
        autoOpenDoor = transform.parent.GetComponentInChildren<AutoOpenDoor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && KeyOutlet.granted)
        {
            autoOpenDoor.OpenDoor();
        }
    }
}
