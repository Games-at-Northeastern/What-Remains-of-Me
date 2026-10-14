using UnityEngine;

public class ScreenGlitchTrigger : MonoBehaviour
{
    public MonitorGlitch monitorGlitch;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            monitorGlitch.StartGlitching();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            monitorGlitch.StopGlitching();
    }
}
