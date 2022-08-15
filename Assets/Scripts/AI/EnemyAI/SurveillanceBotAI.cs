using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BehaviorTreeAI;

public class SurveillanceBotAI : MonoBehaviour
{
    public enum SurveillanceState { Surveillance, Alert, Alarm }

    public GameObject player;
    public SurveillanceLight sLight;
    public float pauseTime;
    public float alertRange;

    private SurveillanceState ss;
    private bool flipStarted = false;

    private void Start()
    {
        ss = SurveillanceState.Surveillance;
    }

    public void Update()
    {
        switch(ss)
        {
            case SurveillanceState.Surveillance:
                StartCoroutine(SurveillanceMode());
                break;
            case SurveillanceState.Alert:
                AlertMode();
                break;
            case SurveillanceState.Alarm:
                AlarmMode();
                break;
        }

        if (sLight.GetCollisionStatus())
        {
            ss = SurveillanceState.Alert;
        }
    }

    public IEnumerator SurveillanceMode()
    {
        if (!flipStarted)
        {
            flipStarted = true;
            yield return new WaitForSeconds(pauseTime);
            this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x * -1f, 1f, 1f);
            flipStarted = false;
        }
    }

    public void AlertMode()
    {
        var enemies = FindObjectsOfType<MonoBehaviour>().OfType<BehaviorTree>();
        foreach (MonoBehaviour enemy in enemies)
        {
            if (Vector3.Distance(this.gameObject.transform.position, enemy.gameObject.transform.position) <= alertRange)
            {
                ((BehaviorTree)enemy).Alerted();
            }
        }

        ss = SurveillanceState.Alarm;
    }

    public void AlarmMode()
    {
        // Just sits there for now
    }
}
