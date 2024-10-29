using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Represents an outlet that allows the player, once connected, to give or take
/// energy for a certain purpose.
/// </summary>
public class Outlet : MonoBehaviour
{
    [SerializeField] protected float maxEnergy;
    [SerializeField] protected float clean;
    [SerializeField] protected float virus;
    [SerializeField] protected float energyTransferSpeed;
    public Collider2D grappleOverrideRange;

    [Header("Outlet Lights")]
    [SerializeField] protected List<Light2D> outletLights;
    [SerializeField] protected float intensityLerpSpeed, connectedIntensity, chargingIntensity;

    [Header("Audio")]
    [SerializeField] protected string plugInSound = "Plug_In";
    [SerializeField] protected string givingChargeSound = "Giving_Charge";
    [SerializeField] protected string takingChargeSound = "Taking_Charge";

    private float goalIntensity = 0f;

    /// <summary>
    /// Makes this outlet function as if the player is connected to it.
    /// </summary>
    public virtual void Connect()
    {
        SoundController.instance.PlaySound(plugInSound);
        goalIntensity = connectedIntensity;
        StartCoroutine(ControlLight());
    }

    /// <summary>
    /// Stops this outlet from functioning as if the player is connected to it.
    /// </summary>
    public virtual void Disconnect()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutLight());
    }

    // Event to be triggered whenever the virus amount changes,
    // sending the current percentage of energy that is virus as a value between 0 and 1
    [NonSerialized]
    public UnityEvent<float> OnVirusChange;
    public void VirusChange(float virusPercentage) => OnVirusChange?.Invoke(virusPercentage);

    [NonSerialized]
    public UnityEvent<float> OnEnergyChange;
    public void EnergyChange(float totalEnergyAmount) => OnEnergyChange?.Invoke(totalEnergyAmount);

    #region lights

    //Lerps the light to the goal
    private IEnumerator ControlLight()
    {
        while (true)
        {
            foreach (Light2D outletLight in outletLights)
            {
                outletLight.intensity = Mathf.Lerp(outletLight.intensity, goalIntensity, Time.deltaTime * intensityLerpSpeed);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    //Fades out the light
    private IEnumerator FadeOutLight()
    {
        while (outletLights[0].intensity > 0.1f)
        {
            foreach (Light2D outletLight in outletLights)
            {
                outletLight.intensity = Mathf.Lerp(outletLight.intensity, 0f, Time.deltaTime * intensityLerpSpeed);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        foreach (Light2D outletLight in outletLights)
        {
            outletLight.intensity = 0f;
        }
    }

    #endregion

    // accepts the max amount of clean and virus energy the caller is willing to give to the outlet,
    // adds some amount of clean and virus energy to this outlet
    // returns the amount of clean and virus energy drained from the caller
    public Vector2 GiveEnergy(float maxClean, float maxVirus)
    {
        StoreEnergyVals();

        float totalMax = maxClean + maxVirus;
        totalMax = Mathf.Min(totalMax, maxEnergy - GetEnergy());
        float givenEnergy = Mathf.Min(totalMax, Time.deltaTime * energyTransferSpeed);

        float givenClean = (maxClean / totalMax) * givenEnergy;
        float givenVirus = (maxVirus / totalMax) * givenEnergy;

        clean += givenClean;
        virus += givenVirus;

        CheckEnergyVals();

        return new Vector2(givenClean, givenVirus);
    }

    // accepts the max amount of energy the energy reciever is willing to accept from the outlet,
    // removes some amount of clean and virus energy from this outlet
    // returns how much clean and virus energy the energy reciever should recieve
    public Vector2 TakeEnergy(float maxRecieverCanTake)
    {
        StoreEnergyVals();

        float takenEnergy = Mathf.Min(maxRecieverCanTake, Time.deltaTime * energyTransferSpeed);

        float takenClean = (clean / GetEnergy()) * takenEnergy;
        float takenVirus = (virus / GetEnergy()) * takenEnergy;

        clean -= takenClean;
        virus -= takenVirus;

        CheckEnergyVals();

        return new Vector2(takenClean, takenVirus);
    }

    // Get the energy level of the controlled object
    public float GetEnergy() => clean + virus;

    public float GetClean() => clean;

    public float GetVirus() => virus;

    public float GetMaxEnergy() => maxEnergy;

    public float GetPercentFull() => GetEnergy() / maxEnergy;

    public float GetPercentVirus() => GetVirus() / maxEnergy;

    // private methods for checking changes in energy

    private float previousEnergy = 0;
    private float previousVirus = 0;
    private float previousVirusPercent = 0;

    private void StoreEnergyVals()
    {
        previousEnergy = GetEnergy();
        previousVirus = GetVirus();
        previousVirusPercent = GetPercentVirus();
    }

    private void CheckEnergyVals()
    {
        if (previousEnergy != GetEnergy())
        {
            EnergyChange(GetEnergy());
        }

        if (previousVirus != GetVirus() || previousVirusPercent != GetPercentVirus())
        {
            VirusChange(GetVirus());
        }
    }
}

