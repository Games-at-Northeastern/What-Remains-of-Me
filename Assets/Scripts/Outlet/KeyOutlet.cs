using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class KeyOutlet : Outlet
{
    [SerializeField] private bool grantOnce = true;
    [SerializeField] private float delayBeforeGrant = 10f;
    [SerializeField] private SpriteRenderer progressDisplay;
    [SerializeField] private List<Sprite> progressSprites;
    [SerializeField] private Animator yellowWarningAnim;
    [SerializeField] private SpriteRenderer yellowWarningSprite;
    [SerializeField] private Animator redWarningAnim;
    [SerializeField] private SpriteRenderer redWarningSprite;
    [SerializeField] private SpriteRenderer completedSprite;
    [SerializeField] private AudioSource audioSourceMonitor;
    [SerializeField] private AudioClip completedDownloadSFX;
    [SerializeField] private AudioClip warningBeepsYellow;
    [SerializeField] private AudioClip warningBeepsRed;

    public WireThrower wire;

    private List<IAlarmListener> listeners = new List<IAlarmListener>();

    public static bool hasKey = false;
    private Coroutine grantRoutine;
    private AudioSource audioSourceSelf;
    
    
    enum ProgressState
    {
        START,
        YELLOW,
        RED,
        END
    }

    private int currentSprite = -1;
    private ProgressState currentProgress = ProgressState.START;

    private void Awake()
    {
        // Set the control scheme for this outlet to start and pause the download when the take energy key is held
        // or released.
        CS = new ControlSchemes();
        CS.Player.TakeEnergy.performed += _ => StartDownload();
        CS.Player.TakeEnergy.canceled += _ => PauseDownload();

        audioSourceSelf = GetComponent<AudioSource>();
    }

    // Start or continue the download
    private void StartDownload()
    {
        audioSourceSelf.Play();
        
        // Begin the coroutine to run the download timer and animation.
        if (!(grantOnce && hasKey))
        {
            grantRoutine = StartCoroutine(RunDownload());
        }
        // Start the warning that was active before the download was last paused, if one had been started.
        if (currentProgress == ProgressState.YELLOW)
        {
            StartYellowWarning();
        }
        else if (currentProgress == ProgressState.RED)
        {
            StartRedWarning();
        }
    }

    // Pause the download by stopping the timer coroutine and the warning animations.
    private void PauseDownload()
    {
        StopAllCoroutines();
        grantRoutine = null;
        StopWarnings();
    }

    // Enable the yellow blinking animation and play the first warning sound.
    private void StartYellowWarning()
    {
        yellowWarningAnim.enabled = true;
        audioSourceMonitor.Stop();

        if (audioSourceMonitor.clip != warningBeepsYellow)
        {
            audioSourceMonitor.clip = warningBeepsYellow;
            audioSourceMonitor.Play();
        }
    }

    // Disable the yellow blinking, enable the red blinking, and play the second warning sound.
    private void StartRedWarning()
    {
        redWarningAnim.enabled = true;
        yellowWarningAnim.enabled = false;
        yellowWarningSprite.enabled = false;
        audioSourceMonitor.Stop();
        
        if (audioSourceMonitor.clip != warningBeepsRed)
        {
            audioSourceMonitor.clip = warningBeepsRed;
            audioSourceMonitor.Play();
        }
    }

    // Stop all warning animations and sounds.
    private void StopWarnings()
    {
        yellowWarningAnim.enabled = false;
        yellowWarningSprite.enabled = false;
        redWarningAnim.enabled = false;
        redWarningSprite.enabled = false;
        audioSourceSelf.Stop();
        audioSourceMonitor.Stop();
    }
    
    // The timer that keeps track of how far the download has progressed, in seconds.
    float timer = 0f;

    // The main download sequence coroutine
    private IEnumerator RunDownload()
    {
        while (timer < delayBeforeGrant)
        {
            timer += Time.deltaTime;
            UpdateAnimation();
            yield return null;
        }

        currentProgress = ProgressState.END;
        StopWarnings();
        hasKey = true;
        audioSourceSelf.PlayOneShot(completedDownloadSFX);

        Invoke("StartAlarms", 1.5f);

        grantRoutine = null;
    }

    // Update the state of the progress bar and warning animations.
    private void UpdateAnimation()
    {
        int progress = (int)((timer / delayBeforeGrant) * (progressSprites.Count - 1));
        if (progress != currentSprite)
        {
            currentSprite = progress;
            progressDisplay.sprite = progressSprites[progress];
        }

        // Start the yellow warning once it reaches 30%.
        if (timer > (delayBeforeGrant * 0.3) && currentProgress == ProgressState.START)
        {
            currentProgress = ProgressState.YELLOW;
            StartYellowWarning();
        }

        // Start the red warning once it reaches 65%
        if (timer > (delayBeforeGrant * 0.65) && currentProgress == ProgressState.YELLOW)
        {
            currentProgress = ProgressState.RED;
            StartRedWarning();
        }

        if(wire)
            wire.showEnergyFlow(-1f);
    }

    // Activate all alarm listeners subscribed to this alarm.
    void StartAlarms()
    {
        foreach (IAlarmListener listener in listeners)
        {
            listener.OnAlarmStart();
        }
    }

    // Subscribe an alarm listener to this alarm.
    public void Subscribe(IAlarmListener listener)
    {
        listeners.Add(listener);
    }
}


