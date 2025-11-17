using System.Collections;
using UnityEngine;

public class KeyDoorTrigger : MonoBehaviour
{
    [SerializeField] private AutoOpenDoor autoOpenDoor;
    [SerializeField] private SpriteRenderer screenRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite checkSprite;
    [SerializeField] private Sprite crossSprite;

    private Coroutine resetRoutine;
    void Start()
    {
        autoOpenDoor = transform.parent.GetComponentInChildren<AutoOpenDoor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && KeyOutlet.granted)
        {
            autoOpenDoor.OpenDoor();
            screenRenderer.sprite = checkSprite;
            return;
        }
        else if (other.CompareTag("Player") && !KeyOutlet.granted)
        {
            screenRenderer.sprite = crossSprite;
            if (resetRoutine != null)
                StopCoroutine(resetRoutine);

            resetRoutine = StartCoroutine(ResetAfterDelay());
        }
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        screenRenderer.sprite = defaultSprite;
        resetRoutine = null;
    }
}
