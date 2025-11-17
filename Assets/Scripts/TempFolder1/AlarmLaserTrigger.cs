using UnityEngine;

public class AlarmLaserTrigger : MonoBehaviour, IAlarmListener
{
    [SerializeField] private DeathLaser laser;
    [SerializeField] private TimedToggle laserToggle;
    [SerializeField] private KeyOutlet keyOutlet;

    void Start()
    {
        keyOutlet.Subscribe(this);
        laser.ToggleLaser();
    }

    public void OnAlarmStart()
    {
        laserToggle.enabled = true;
    }
}
