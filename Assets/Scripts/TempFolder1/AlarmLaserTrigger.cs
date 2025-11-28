using UnityEngine;
using System.Collections;

public class AlarmLaserTrigger : MonoBehaviour, IAlarmListener
{
    [SerializeField] private DeathLaser laser;
    [SerializeField] private TimedToggle laserToggle;
    [SerializeField] private KeyOutlet keyOutlet;
    [SerializeField] private bool isToggle = true;

    void Start()
    {

        keyOutlet.Subscribe(this);
        if(laser)
            laser.ToggleLaser();
    }

    public void OnAlarmStart()
    {

        StartCoroutine(WaitToTurnOn());
    }

    // Activates the alarm lights after a set of time
    private IEnumerator WaitToTurnOn() 
    {
        yield return new WaitForSeconds(2f);
        if(laserToggle && isToggle)
            laserToggle.enabled = true;
        else if (laser)
            laser.ToggleLaser();
    }
}
    
