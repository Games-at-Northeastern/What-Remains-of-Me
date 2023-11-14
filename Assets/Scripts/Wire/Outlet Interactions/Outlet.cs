using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an outlet that allows the player, once connected, to give or take
/// energy for a certain purpose.
/// </summary>
public class Outlet : MonoBehaviour
{
    //private SoundController soundController;
    [SerializeField] private SoundArray OutletSounds;

    [Header("SFX")]
    public AudioSource src;
    public AudioClip giving;
    public AudioClip taking;

    ControlSchemes CS;
    [SerializeField] AControllable controlled;
    [SerializeField] List<AControllable> controlledSecondaries;
    [SerializeField] float energyTransferSpeed;

    public Collider2D grappleOverrideRange;

    private void Awake()
    {
        // TODO : This should be moved into one of the player scripts
        CS = new ControlSchemes();
        CS.Player.GiveEnergy.performed += _ => { if (controlled != null) { StartCoroutine("GiveEnergy"); } };
        CS.Player.TakeEnergy.performed += _ => { if (controlled != null) { StartCoroutine("TakeEnergy"); } };
        CS.Player.GiveEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("GiveEnergy"); src.Stop(); } };
        CS.Player.TakeEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("TakeEnergy"); src.Stop(); } };

        CS.Player.GiveVirus.performed += _ => { if (controlled != null) { StartCoroutine("GiveVirus"); } };
        CS.Player.TakeVirus.performed += _ => { if (controlled != null) { StartCoroutine("TakeVirus"); } };
        CS.Player.GiveVirus.canceled += _ => { if (controlled != null) { StopCoroutine("GiveVirus"); src.Stop(); } };
        CS.Player.TakeVirus.canceled += _ => { if (controlled != null) { StopCoroutine("TakeVirus"); src.Stop(); } };

        //soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    /// <summary>
    /// Makes this outlet function as if the player is connected to it.
    /// </summary>
    public void Connect()
    {
        CS.Enable();
        src.PlayOneShot(OutletSounds.GetSound("Plug_In"));
        //soundController.PlaySound("Plug_In");
    }

    /// <summary>
    /// Stops this outlet from functioning as if the player is connected to it.
    /// </summary>
    public void Disconnect()
    {
        CS.Disable();
        StopAllCoroutines();
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
            src.clip = giving;
            src.Play();
        }
    }

    /// <summary>
    /// Gives virus to the controlled object until this coroutine is called to end.
    /// </summary>
    IEnumerator GiveVirus()
    {
        while (true)
        {
            controlled.GainVirus(energyTransferSpeed * Time.deltaTime);
            foreach (AControllable cSec in controlledSecondaries)
            {
                if (cSec != null)
                {
                    cSec.GainVirus(energyTransferSpeed * Time.deltaTime);
                }
            }
            yield return new WaitForEndOfFrame();

            // SFX
            src.clip = giving;
            src.Play();
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
            src.clip = taking;
            src.Play();
        }
    }

    /// <summary>
    /// Takes virus from the controlled object until this coroutine is called to end.
    /// </summary>
    IEnumerator TakeVirus()
    {
        while (true)
        {
            controlled.LoseVirus(energyTransferSpeed * Time.deltaTime);
            foreach (AControllable cSec in controlledSecondaries)
            {
                if (cSec != null)
                {
                    cSec.LoseVirus(energyTransferSpeed * Time.deltaTime);
                }
            }
            yield return new WaitForEndOfFrame();

            // SFX
            src.clip = taking;
            src.Play();
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
            return maxCharge + controlled.GetMaxCharge();
        }
        return 0f;
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
}

