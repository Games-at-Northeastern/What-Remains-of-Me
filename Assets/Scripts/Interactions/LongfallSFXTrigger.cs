using UnityEngine;

public class LongfallSFXTrigger : MonoBehaviour
{
    [SerializeField] private string longfallSFX = "Longfall";
    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasPlayed)
        {
            return;
        }
        if (collision.CompareTag("Player"))
        {
            hasPlayed = true;
            Debug.Log("Player entered trigger");
            SoundController.instance.PlaySound(longfallSFX);
        }
    }
}
