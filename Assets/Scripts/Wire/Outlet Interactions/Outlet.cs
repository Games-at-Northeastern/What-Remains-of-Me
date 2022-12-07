using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an outlet that allows the player, once connected, to give or take
/// energy for a certain purpose.
/// </summary>
public class Outlet : MonoBehaviour
{
    private SoundController soundController;

    [Header("SFX")]
    public AudioSource src;
    public AudioClip giving;
    public AudioClip taking;

    ControlSchemes CS;
    [SerializeField] AControllable controlled;
    [SerializeField] float energyTransferSpeed;

    private void Awake()
    {
        CS = new ControlSchemes();
        CS.Player.GiveEnergy.performed += _ => { if (controlled != null) { StartCoroutine("GiveEnergy"); } };
        CS.Player.TakeEnergy.performed += _ => { if (controlled != null) { StartCoroutine("TakeEnergy"); } };
        CS.Player.GiveEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("GiveEnergy"); } };
        CS.Player.TakeEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("TakeEnergy"); } };

        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    /// <summary>
    /// Makes this outlet function as if the player is connected to it.
    /// </summary>
    public void Connect()
    {
        CS.Enable();
        soundController.PlaySound("Plug_In");
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
            yield return new WaitForEndOfFrame();

            // SFX
            src.clip = taking;
            src.Play();
        }
    }
}

