using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RechargeLeakBatteryPack : Outlet
{
    [SerializeField] bool charge;
    [SerializeField, Range(0f, 1f)] float virusRatio;
    private bool playerConnected = false;

    [SerializeField] Effects effects;

    private void Update()
    {
        if (charge)
        {
            this.GiveEnergy(Time.deltaTime * energyTransferSpeed * (1 - virusRatio), Time.deltaTime * energyTransferSpeed * virusRatio);
            if (GetPercentFull() < .98f)
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
            this.TakeEnergy(Time.deltaTime * energyTransferSpeed);
            if (GetPercentFull() > 0)
            {
                effects.PlayEffect();
            }
            else
            {
                effects.CancelEffect();
            }
        }
    }
}
