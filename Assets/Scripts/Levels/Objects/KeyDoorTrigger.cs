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
    private ControlSchemes cs;

    void Start()
    {
        autoOpenDoor = transform.parent.GetComponentInChildren<AutoOpenDoor>();
        cs = new ControlSchemes();
        cs.Player.Dialogue.performed += _ => EnterCode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            screenRenderer.sprite = crossSprite;
            moniterAudioSource.PlayOneShot(denySFX);
            cs.Enable();
        }
    }

    private void EnterCode()
    {
        if (KeyOutlet.hasKey)
        {
            autoOpenDoor.OpenDoor();
            screenRenderer.sprite = checkSprite;
            moniterAudioSource.PlayOneShot(acceptSFX);
            cs.Disable();
            Destroy(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        screenRenderer.sprite = defaultSprite;
        cs.Disable();
    }
}
