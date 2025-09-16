using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This script handles the visuals of the energy level contained within an outlet.
///
/// To assign a new sprite sheet to this visual, go to the inspector and select each sprite in the sliced spritesheet,
/// then drag the selection into the serialized array in the inspector.
/// </summary>
public class OutletMeter : MonoBehaviour
{
    private Outlet outlet;

    // Sprite arrays for different meter states
    [SerializeField] private Sprite[] limiterSprites;
    [SerializeField] private Sprite[] virusSprites;
    [SerializeField] private Sprite[] cleanSprites;

    // Sprite renderers for the meter visuals
    [SerializeField] private SpriteRenderer limiterMeter;
    [SerializeField] private SpriteRenderer virusMeter;
    [SerializeField] private SpriteRenderer cleanMeter;

    [SerializeField] private float visualFillSpeed = 30;
    [SerializeField] private float endLimits = 0.2f;

    // the value represented by a full outlet with no limiters,
    // no outlet should store a max charge higher than this
    private float generalMaxOutletCharge = 100f;

    private int numEnergyTicks;

    private float targetVirus;
    private float targetClean;

    private float currentVirus;
    private float currentClean;

    private float actualVirus;
    private float actualClean;

    private float maxCharge;

    private bool plugConnected;
    private bool updatingVisuals = false;
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

    public bool visualsAlwaysDisplayed; // If true, meter sprite visuals will always display
    private bool isDisplayed; // Don't want to restart the visuals if they're already displayed

    private void Awake()
    {
        outlet = transform.GetComponentInParent<Outlet>();

        numEnergyTicks = cleanSprites.Length;

        maxCharge = outlet.GetMaxCharge();
        GetValues();

        if (visualsAlwaysDisplayed)
        {
            StartVisuals();
        }
    }

    private void LateUpdate()
    {
        if (updatingVisuals)
        {
            GetValues();
            
            if (!powered)
            {
                targetVirus = 0;
                targetClean = 0;
            }
            else
            {
                targetVirus = actualVirus;
                targetClean = actualClean;
            }

            

            //currentVirus = Mathf.Lerp(currentVirus, targetVirus, visualFillSpeed * Time.deltaTime);
            //currentClean = Mathf.Lerp(currentClean, targetClean, visualFillSpeed * Time.deltaTime);

            currentVirus = Mathf.MoveTowards(currentVirus, targetVirus, visualFillSpeed * Time.deltaTime);
            currentClean = Mathf.MoveTowards(currentClean, targetClean, visualFillSpeed * Time.deltaTime);

            VirusState = calculateEnergyState(currentVirus);
            CleanState = calculateEnergyState(currentClean);

            virusMeter.sprite = virusSprites[_virusState];
            cleanMeter.sprite = cleanSprites[Mathf.Min(_cleanState + _virusState, cleanSprites.Length - 1)];

            // Check if both energy and virus levels are low and not powered, then stop the coroutine
            if (currentClean < endLimits && currentVirus < endLimits && !powered)
            {
                virusMeter.sprite = virusSprites[0];
                cleanMeter.sprite = cleanSprites[0];
                currentVirus = 0;
                currentClean = 0;
                updatingVisuals = false;
                StopAllCoroutines();
            }
        }
    }

    // Get the initial values from the associated Outlet object
    public void GetValues()
    {
        if (outlet == null)
        { return; }

        float limiterAmount = ((generalMaxOutletCharge - maxCharge) / generalMaxOutletCharge) * limiterSprites.Length - 1;
        LimiterState = Mathf.FloorToInt(limiterAmount);

        actualVirus = outlet.GetVirus();
        actualClean = outlet.GetEnergy();

        limiterMeter.sprite = limiterSprites[_limiterState];
    }

    // Start the visuals update coroutine
    public void StartVisuals()
    {
        if (!isDisplayed)
        {
            powered = true;
            isDisplayed = true;
            if (updatingVisuals || plugConnected)
            { return; }
            updatingVisuals = true;
        }
    }

    // End the visuals update coroutine
    public void EndVisuals()
    {
        if (!visualsAlwaysDisplayed)
        {
            if (plugConnected)
            { return; }
            powered = false;
            isDisplayed = false;
        }
    }

    // Connect the plug to the outlet
    public void ConnectPlug()
    {
        plugConnected = true;
        powered = true;
    }

    // Disconnect the plug from the outlet
    public void DisconnectPlug()
    {
        plugConnected = false;
        powered = false;
    }

    private int calculateEnergyState(float current)
    {
        if (current < endLimits)
        {
            return 0;
        }

        int maxEnergyTicks = Mathf.CeilToInt((maxCharge / generalMaxOutletCharge) * numEnergyTicks);

        if (maxEnergyTicks % 2 == 0) // ensures maxEnergyTicks is Odd, therefore the max energy tick is even
                                     // because currently, odd-indexed charge states are incomplete, and a full battery must end on a complete sprite
        {
            maxEnergyTicks += 1;
        }

        float tickSize = (maxCharge - (2 * endLimits)) / (maxEnergyTicks - 2);

        current -= endLimits;

        return Mathf.CeilToInt(current / tickSize);

    }
}

