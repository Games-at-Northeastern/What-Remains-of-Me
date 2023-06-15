using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This script handles the visuals of the energy level contained within an outlet.
///
/// To assign new a new sprite sheet to this visual, go to the inspector and select each sprite in the sliced spritesheet,
/// then drag the selection into the serialized array in the inspector.
/// </summary>
public class OutletMeter : MonoBehaviour
{

    private Outlet outlet;

    [SerializeField] private Sprite[] limiterSprites;
    [SerializeField] private Sprite[] virusSprites;
    [SerializeField] private Sprite[] cleanSprites;

    [SerializeField] private SpriteRenderer limiterMeter;
    [SerializeField] private SpriteRenderer virusMeter;
    [SerializeField] private SpriteRenderer cleanMeter;

    [SerializeField] private float visualFillSpeed = 1;

    private float targetVirus;
    private float targetClean;

    private float currentVirus;
    private float currentClean;

    private float actualVirus;
    private float actualClean;

    private bool plugConnected;
    private bool coroutineRunning = false;
    private bool powered = false;

    private int _limiterState = 0;

    private int LimiterState
    {
        get { return _limiterState; }
        set { _limiterState = Mathf.Clamp(value, 0, limiterSprites.Length - 1); }
    }

    private int _virusState = 0;
    private int VirusState
    {
        get { return _virusState; }
        set { _virusState = Mathf.Clamp(value, 0, virusSprites.Length - 1); }
    }

    private int _cleanState = 0;

    private int CleanState
    {
        get { return _cleanState; }
        set { _cleanState = Mathf.Clamp(value, 0, cleanSprites.Length - 1); }
    }

    private void Awake()
    {
        outlet = transform.GetComponentInParent<Outlet>();
        GetValues();
    }

    public void GetValues()
    {
        if (outlet == null) { return; }

        float limiterAmount = ((100 - outlet.GetMaxCharge()) / 100) * limiterSprites.Length - 1;
        LimiterState = Mathf.FloorToInt(limiterAmount);

        actualVirus = outlet.GetVirus();
        actualClean = outlet.GetEnergy();

        limiterMeter.sprite = limiterSprites[_limiterState];
    }

    public void StartVisuals()
    {
        if (coroutineRunning || plugConnected)
        { return; }
        Debug.Log("startVisuals");
        StartCoroutine(UpdateVisuals());
    }

    public void EndVisuals()
    {
        if (plugConnected)
        { return; }
        Debug.Log("endVisuals");
        powered = false;
    }

    public void ConnectPlug()
    {
        plugConnected = true;
    }

    public void DisconnectPlug()
    {
        plugConnected = false;
    }

    private IEnumerator UpdateVisuals()
    {
        Debug.Log("Starting outletmeter coroutine");
        coroutineRunning = true;
        powered = true;
        while (true)
        {
            GetValues();
            if(!powered)
            {
                targetVirus = 0;
                targetClean = 0;
            } else
            {
                targetVirus = actualVirus;
                targetClean = actualClean;
            }

            currentVirus = Mathf.Lerp(currentVirus, targetVirus, visualFillSpeed * Time.deltaTime);
            currentClean = Mathf.Lerp(currentClean, targetClean, visualFillSpeed * Time.deltaTime);

            VirusState = Mathf.FloorToInt((currentVirus / 12.5f) * 2);
            CleanState = Mathf.FloorToInt((currentClean / 12.5f) * 2);
            if (currentVirus > 0.05f && VirusState == 0)
            {
                VirusState = 1;
            }
            if (currentClean > 0.05f && CleanState == 0)
            {
                CleanState = 1;
            }

            virusMeter.sprite = virusSprites[_virusState];
            cleanMeter.sprite = cleanSprites[Mathf.Min(_cleanState + _virusState, cleanSprites.Length - 1)];

            if (currentClean < 6f && currentVirus < 6f && !powered)
            {
                virusMeter.sprite = virusSprites[0];
                cleanMeter.sprite = cleanSprites[0];
                currentVirus = 0;
                currentClean = 0;
                coroutineRunning = false;
                Debug.Log("Stopping outletmeter coroutine");
                StopAllCoroutines();
            }

            yield return null;
        } 
    }
}
