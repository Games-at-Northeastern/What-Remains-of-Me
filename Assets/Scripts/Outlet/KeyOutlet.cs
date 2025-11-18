using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

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

    public WireThrower wire;

    private List<IAlarmListener> listeners = new List<IAlarmListener>();

    public static bool hasKey = false;
    private Coroutine grantRoutine;
    
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
        CS = new ControlSchemes();
        CS.Player.TakeEnergy.performed += _ => StartDownload();
        CS.Player.TakeEnergy.canceled += _ => PauseDownload();
    }

    private void StartDownload()
    {
        if (!(grantOnce && hasKey))
        {
            grantRoutine = StartCoroutine(GetKey());
        }
        if (currentProgress == ProgressState.YELLOW)
        {
            yellowWarningAnim.enabled = true;
        }
        else if (currentProgress == ProgressState.RED)
        {
            redWarningAnim.enabled = true;
            yellowWarningAnim.enabled = false;
            yellowWarningSprite.enabled = false;
        }
    }

    private void PauseDownload()
    {
        StopAllCoroutines();
        grantRoutine = null;
        yellowWarningAnim.enabled = false;
        yellowWarningSprite.enabled = false;
        redWarningAnim.enabled = false;
        redWarningSprite.enabled = false;
    }
    
    float timer = 0f;

    private IEnumerator GetKey()
    {
        while (timer < delayBeforeGrant)
        {
            timer += Time.deltaTime;
            int progress = (int)((timer / delayBeforeGrant) * (progressSprites.Count - 1));
            if (progress != currentSprite)
            {
                currentSprite = progress;
                progressDisplay.sprite = progressSprites[progress];
            }

            if (timer > (delayBeforeGrant * 0.3) && currentProgress == ProgressState.START)
            {
                currentProgress = ProgressState.YELLOW;
                yellowWarningAnim.enabled = true;
            }

            if (timer > (delayBeforeGrant * 0.65) && currentProgress == ProgressState.YELLOW)
            {
                currentProgress = ProgressState.RED;
                redWarningAnim.enabled = true;
                yellowWarningAnim.enabled = false;
                yellowWarningSprite.enabled = false;
            }
            if(wire)
                wire.showEnergyFlow(-1f);

            yield return null;
        }

        currentProgress = ProgressState.END;
        yellowWarningAnim.enabled = false;
        yellowWarningSprite.enabled = false;
        redWarningAnim.enabled = false;
        redWarningSprite.enabled = false;
        hasKey = true;
        Debug.Log("Player has key! ");
        

        OnDownloaded();

        grantRoutine = null;
    }

    private void OnDownloaded()
    {
        // Start the animation for when the download is finished here

        // Move this code to be triggered by the animation when you want the lasers and lights to turn on
        // (probably after the camera has panned out)
        Invoke("StartAlarms", 1.5f);
    }

    void StartAlarms()
    {
        foreach (IAlarmListener listener in listeners)
        {
            listener.OnAlarmStart();
        }
    }

    public void Subscribe(IAlarmListener listener)
    {
        listeners.Add(listener);
    }
}


