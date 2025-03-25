using System.Collections.Generic;
using UnityEngine;

public class OutletLock : MonoBehaviour
{
    [SerializeField] private List<Outlet> outlets;
    [SerializeField] private List<OutletMeter> meters;
    [SerializeField] private MovingElementController controller;
    private bool hasLocked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.GetMaxCharge() <= controller.GetEnergy() && !hasLocked)
        {
            hasLocked = true;
            foreach (Outlet outlet in outlets)
            {

                outlet.Lock(true);
            }
            foreach (OutletMeter meter in meters)
            {
                meter.GetValues();
            }
        }
    }

}
