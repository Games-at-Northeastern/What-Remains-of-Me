using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeLeakBatteryPack : AControllable
{
    [SerializeField] Boolean fillBattery;
    [SerializeField] protected float maxCharge;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void private void FixedUpdate() {
        if (fillBattery) {
            chargeBattery();
        }
        else {
            drainBattery();
        }
    }

    void chargeBattery() {
        if (totalEnergy >= maxCharge/2 & playerInfo == null){
            totalEnergy += Time.deltaTime;
        }
    }

    void drainBattery() {
        if (totalEnergy >= 0 & playerInfo == null){
            totalEnergy -= Time.deltaTime;
        }
    }

    
}
