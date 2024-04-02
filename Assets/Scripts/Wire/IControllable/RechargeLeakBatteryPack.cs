using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeLeakBatteryPack : Outlet
{
    [SerializeField] bool charge;
    [SerializeField] float virusRatio;
    private bool playerConnected = false;

    private void Awake()
    {
        CS = new ControlSchemes();
        CS.Player.GiveEnergy.performed += _ => { if (controlled != null) { StartCoroutine("GiveEnergy"); } };
        CS.Player.TakeEnergy.performed += _ => { if (controlled != null) { StartCoroutine("TakeEnergy"); } };
        CS.Player.GiveEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("GiveEnergy"); SoundController.instance.StopSound(givingChargeSound); } };
        CS.Player.TakeEnergy.canceled += _ => { if (controlled != null) { StopCoroutine("TakeEnergy"); SoundController.instance.StopSound(takingChargeSound); } };

     }

    private void FixedUpdate() {
        if (!playerConnected) {
            if(charge) {
                controlled.CreateEnergy(energyTransferSpeed * Time.deltaTime/3, virusRatio);
            } else {
                controlled.LeakEnergy(energyTransferSpeed * Time.deltaTime/3);
            }
        }
    }

    public override void Connect()
    {
        CS.Enable();
        SoundController.instance.PlaySound(plugInSound);
        playerConnected = true;
    }

    public override void Disconnect()
    {
        playerConnected = false;
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
