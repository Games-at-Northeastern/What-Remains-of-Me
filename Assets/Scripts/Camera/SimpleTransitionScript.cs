using UnityEngine;

// Make sure the GameObject has the necessary components!
[RequireComponent(typeof(Cinemachine.CinemachineVirtualCamera), typeof(Collider2D))]
public class SimpleTransitionScript : MonoBehaviour {
    private Cinemachine.CinemachineVirtualCamera cam;

    // Gets the camera from the current GameObject
    private void Awake() => cam = GetComponent<Cinemachine.CinemachineVirtualCamera>();

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) /* If the player entered the camera */
        {
            cam.Priority = 100;  // Make the priority of the camera very high
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) /* If the player left the camera */
        {
            cam.Priority = -100; // Make the priority of the camera very low
        }
    }
}
