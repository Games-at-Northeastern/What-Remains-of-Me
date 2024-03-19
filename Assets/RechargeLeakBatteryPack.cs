using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeLeakBatteryPack : AControllable
{
    [SerializeField] bool fillBattery;
    [SerializeField] protected float maxCharge;

    // Update is called once per frame
    private void FixedUpdate() {
        if (fillBattery) {
            chargeBattery();
        }
        else {
            drainBattery();
        }
    }

    void chargeBattery() {
        if (totalEnergy >= maxCharge & playerInfo == null){
            //totalEnergy += Time.deltaTime;  //Does not work (totalEnergy is protected)
        }
    }

    void drainBattery() {
        if (totalEnergy >= 0 & playerInfo == null){
            //totalEnergy -= Time.deltaTime; Does not work (totalEnergy is protected)
        }
    }
}
