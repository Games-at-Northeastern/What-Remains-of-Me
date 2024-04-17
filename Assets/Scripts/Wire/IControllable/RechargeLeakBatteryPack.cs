using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RechargeLeakBatteryPack : Outlet
{
    [SerializeField] bool charge;
    [SerializeField] float virusRatio;
    private bool playerConnected = false;

    [SerializeField] Effects effects;

    private void FixedUpdate()
    {
        // if (!playerConnected)
        // {
            if (charge)
            {
                controlled.CreateEnergy(energyTransferSpeed * Time.deltaTime/3, virusRatio);
                if (controlled.GetPercentFull() < .98f)
                {
                    effects.PlayEffect();
                }
                else
                {
                    effects.CancelEffect();
                }
            }
            else
            {
                controlled.LeakEnergy(energyTransferSpeed * Time.deltaTime/3);
                if (controlled.GetPercentFull() > 0)
                {
                    effects.PlayEffect();
                }
                else
                {
                    effects.CancelEffect();
                }
            }
        // }
    }

    public override void Connect()
    {
        CS.Enable();
        SoundController.instance.PlaySound(plugInSound);
        playerConnected = true;
        goalIntensity = connectedGoal;
        StartCoroutine(ControlLight());
    }

    public override void Disconnect()
    {
        playerConnected = false;
        CS.Disable();
        StopAllCoroutines();
        StartCoroutine(FadeOutLight());
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
}
