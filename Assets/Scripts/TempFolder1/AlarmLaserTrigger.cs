using UnityEngine;

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
        if(laserToggle && isToggle)
            laserToggle.enabled = true;
        else if (laser)
            laser.ToggleLaser();
    }
}
