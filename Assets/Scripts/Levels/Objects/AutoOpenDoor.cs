using UnityEngine;

public class AutoOpenDoor : MonoBehaviour
{
    public bool HasKey { get; private set; } = false;

    public float openHeight = 3f;
    public float openSpeed = 2f;

    private bool isOpening = false;
    private bool isOpened = false;
    private Vector3 closedPos;
    private Vector3 openPos;
    [SerializeField] private AudioSource openingAudioLoop;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
        openingAudioLoop = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isOpened)
        {
            if (openingAudioLoop && openingAudioLoop.isPlaying)
                openingAudioLoop.Stop();
            return;
        }

        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPos, openSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, openPos) < 0.01f)
        {
            isOpened = true;
            isOpening = false;
        }
    }

    public void OpenDoor()
    {
        isOpening = true;
        if (openingAudioLoop)
        {
            openingAudioLoop.Play();
        }
        Debug.Log("Door is opening!");
    }

    public void SetHasKey()
    {
        HasKey = true;
    }
}
