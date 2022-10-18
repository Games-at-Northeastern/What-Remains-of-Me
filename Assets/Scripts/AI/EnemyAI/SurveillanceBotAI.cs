using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the SurveilanceBot's AI and supports its functionality
/// </summary>
public class SurveillanceBotAI : MonoBehaviour
{
    public enum SurveillanceState { Surveillance, Alert, Alarm }

    public GameObject player;
    public SurveillanceLight sLight;
    public float pauseTime;
    public float alertRange;

    private SurveillanceState ss;
    private bool flipStarted = false;

    /// <summary>
    /// Initialize the SurveillanceAI set to the surveillance state
    /// </summary>
    private void Start()
    {
        ss = SurveillanceState.Surveillance;
    }

    /// <summary>
    /// Every frame, check what state the AI is in and react accordingly.
    /// If there is a collision set the AI t Surveilance state Alarmed 
    /// </summary>
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

    /// <summary>
    /// Represents Surveilnce mode and changes the scale
    /// of the transform relative to the player.
    /// </summary>
    /// <returns>Wait period after the frame</returns>
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

    /// <summary>
    /// Represents when the AI is alerted, scans for enemies within range
    /// and alerts them.
    /// </summary>
    public void AlertMode()
    {
        var enemies = FindObjectsOfType<MonoBehaviour>().OfType<IBehaviorTree>();
        foreach (MonoBehaviour enemy in enemies)
        {
            if (Vector3.Distance(this.gameObject.transform.position, enemy.gameObject.transform.position) <= alertRange)
            {
                ((IBehaviorTree)enemy).Alerted();
            }
        }

        ss = SurveillanceState.Alarm;
    }

    /// <summary>
    /// Represents Alarm mode but right now is unimplemented.
    /// </summary>
    public void AlarmMode()
    {
        // Just sits there for now
    }
}
