using UnityEngine;

public class KeyDoorTrigger : MonoBehaviour
{
    public AutoOpenDoor autoOpenDoor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && autoOpenDoor.HasKey)
        {
            autoOpenDoor.OpenDoor();
        }
    }
}
