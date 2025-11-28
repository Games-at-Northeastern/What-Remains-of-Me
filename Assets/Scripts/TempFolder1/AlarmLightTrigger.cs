using UnityEngine;
using UnityEngine.Rendering.Universal;  
using System.Collections;

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
        StartCoroutine(WaitToTurnOn());
    
    }

    // Activates the alarm lights after a set of time
    private IEnumerator WaitToTurnOn() 
    {
        yield return new WaitForSeconds(2f);
        alarmLight.enabled = true;
        alarmAnimation.enabled = true;
    }
}
