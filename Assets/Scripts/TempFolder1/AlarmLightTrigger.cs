using UnityEngine;
using UnityEngine.Rendering.Universal;  

public class AlarmLightTrigger : MonoBehaviour, IAlarmListener

{

    [SerializeField] private Animator alarmAnimation;
    [SerializeField] private Light2D alarmLight;
    [SerializeField] private KeyOutlet keyOutlet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alarmAnimation.enabled = false;
        alarmLight.enabled = false;
        keyOutlet.Subscribe(this);
        
    }

    public void OnAlarmStart()
    {
        alarmLight.enabled = true;
        alarmAnimation.enabled = true;
    }
}
