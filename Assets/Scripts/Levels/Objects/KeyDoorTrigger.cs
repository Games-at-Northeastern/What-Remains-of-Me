using System.Collections;
using UnityEngine;

public class KeyDoorTrigger : MonoBehaviour
{
    [SerializeField] private AutoOpenDoor autoOpenDoor;
    [SerializeField] private SpriteRenderer screenRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite checkSprite;
    [SerializeField] private Sprite crossSprite;
    [SerializeField] private AudioSource moniterAudioSource;
    [SerializeField] private AudioClip denySFX;
    [SerializeField] private AudioClip acceptSFX;

    private Coroutine resetRoutine;

    void Start()
    {
        autoOpenDoor = transform.parent.GetComponentInChildren<AutoOpenDoor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && KeyOutlet.hasKey)
        {
            autoOpenDoor.OpenDoor();
            screenRenderer.sprite = checkSprite;
            moniterAudioSource.PlayOneShot(acceptSFX);
            Destroy(this);
            return;
        }
        else if (other.CompareTag("Player") && !KeyOutlet.hasKey)
        {
            screenRenderer.sprite = crossSprite;
            moniterAudioSource.PlayOneShot(denySFX);
            /*if (resetRoutine != null)
                StopCoroutine(resetRoutine);*/

            //resetRoutine = StartCoroutine(ResetAfterDelay());
        }
    }

    private void OnTriggerExit2D(Collider2D other) => screenRenderer.sprite = defaultSprite;

    /*
    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        screenRenderer.sprite = defaultSprite;
        resetRoutine = null;
    }
    */
}
