using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Represents an outlet that allows the player, once connected, to give or take
/// energy for a certain purpose.
/// </summary>
public class Outlet : MonoBehaviour
{

    protected ControlSchemes CS;
    [SerializeField] public AControllable controlled;
    [SerializeField] protected List<AControllable> controlledSecondaries;
    [SerializeField] protected float energyTransferSpeed;
    [SerializeField] protected List<Light2D> outletLights;
    [SerializeField] protected float lerpSpeed, connectedGoal, chargingGoal, targetingGoal;

    public Collider2D grappleOverrideRange;

    protected string plugInSound = "Plug_In";
    protected string givingChargeSound = "Giving_Charge";
    protected string takingChargeSound = "Taking_Charge";

    float goalIntensity = 0f;

    private void Awake()
    {
        // TODO : This should be moved into one of the player scripts
        CS = new ControlSchemes();
        CS.Player.GiveEnergy.performed += _ => { if (controlled != null) { StartCoroutine("GiveEnergy"); goalIntensity = chargingGoal; } };
        CS.Player.TakeEnergy.performed += _ => { if (controlled != null) { StartCoroutine("TakeEnergy"); goalIntensity = chargingGoal; } };
        CS.Player.GiveEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("GiveEnergy"); SoundController.instance.StopSound(givingChargeSound); goalIntensity = connectedGoal; } };
        CS.Player.TakeEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("TakeEnergy"); SoundController.instance.StopSound(takingChargeSound); goalIntensity = connectedGoal; } };

        //soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        controlled.uniqueID = gameObject.GetComponent<UniqueID>().uniqueId;
    }

    /// <summary>
    /// Makes this outlet function as if the player is connected to it.
    /// </summary>
    public virtual void Connect()
    {
        CS.Enable();
        SoundController.instance.PlaySound(plugInSound);
        goalIntensity = connectedGoal;
        StartCoroutine(ControlLight());
        //src.PlayOneShot(OutletSounds.GetSound("Plug_In"));
        //soundController.PlaySound("Plug_In");
    }

    /// <summary>
    /// Stops this outlet from functioning as if the player is connected to it.
    /// </summary>
    public virtual void Disconnect()
    {
        CS.Disable();
        StopAllCoroutines();
        StartCoroutine(FadeOutLight());
    }

    //Lerps the light to the goal
    IEnumerator ControlLight()
    {
        while (true)
        {
            foreach (Light2D outletLight in outletLights)
                outletLight.intensity = Mathf.Lerp(outletLight.intensity, goalIntensity, Time.deltaTime * lerpSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    //Fades out the light
    IEnumerator FadeOutLight()
    {
        while (outletLights[0].intensity > 0.1f)
        {
            foreach (Light2D outletLight in outletLights)
                outletLight.intensity = Mathf.Lerp(outletLight.intensity, 0f, Time.deltaTime * lerpSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        foreach (Light2D outletLight in outletLights)
            outletLight.intensity = 0f;
    }
    /// <summary>
    /// Gives energy to the controlled object until this coroutine is called to end.
    /// </summary>
    IEnumerator GiveEnergy()
    {
        while (true)
        {
            controlled.GainEnergy(energyTransferSpeed * Time.deltaTime);
            foreach(AControllable cSec in controlledSecondaries)
            {
                if (cSec != null)
                {
                    cSec.GainEnergy(energyTransferSpeed * Time.deltaTime);
                }
            }
            yield return new WaitForEndOfFrame();

            // SFX
            SoundController.instance.PlaySound(givingChargeSound);
        }
    }

    /// <summary>
    /// Takes energy from the controlled object until this coroutine is called to end.
    /// </summary>
    IEnumerator TakeEnergy()
    {
        while (true)
        {
            controlled.LoseEnergy(energyTransferSpeed * Time.deltaTime);
            foreach (AControllable cSec in controlledSecondaries)
            {
                if (cSec != null)
                {
                    cSec.LoseEnergy(energyTransferSpeed * Time.deltaTime);
                }
            }
            yield return new WaitForEndOfFrame();

            // SFX
            SoundController.instance.PlaySound(takingChargeSound);
        }
    }

    // Get the maximum charge of the controlled object
    public float GetMaxCharge()
    {
        float maxCharge = 0f;
        if (controlled != null)
        {
            foreach (AControllable cSec in controlledSecondaries)
            {
                if (cSec != null)
                {
                    maxCharge += cSec.GetMaxCharge(); 
                }
            }
            maxCharge += controlled.GetMaxCharge();
        }
        return maxCharge;
    }

    // Get the energy level of the controlled object
    public float GetEnergy()
    {
        float energy = 0f;
        if (controlled != null)
        {
            foreach (AControllable cSec in controlledSecondaries)
            {
                if (cSec != null)
                {
                    energy += cSec.GetEnergy();
                }
            }
            return energy + controlled.GetEnergy();
        }
        return 0f;
    }

    // Get the virus level of the controlled object
    public float GetVirus()
    {
        float virus = 0f;
        if (controlled != null)
        {
            foreach (AControllable cSec in controlledSecondaries)
            {
                if (cSec != null)
                {
                    virus += cSec.GetVirus();
                }
            }
            return virus + controlled.GetVirus();
        }
        return 0f;
    }
    /// <summary>
    /// Sets the outlet as targeted, increasing the brightness of the lights.
    /// </summary>
    public void SetTargeted(bool isTargeted)
    {
        goalIntensity = isTargeted ? chargingGoal : connectedGoal;
        StopAllCoroutines();
        StartCoroutine(ControlLight());
    }
}

