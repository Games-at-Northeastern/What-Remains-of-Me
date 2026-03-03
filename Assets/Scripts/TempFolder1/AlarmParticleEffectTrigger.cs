using UnityEngine;
using System.Collections;

public class AlarmParticleEffectTrigger : MonoBehaviour, IAlarmListener
{
    [SerializeField] private TimedToggle toggle;
    [SerializeField] private KeyOutlet keyOutlet;

    void Start()
    {

        keyOutlet.Subscribe(this);
        if (toggle)
            toggle.enabled = false;
    }

    public void OnAlarmStart()
    {

        StartCoroutine(WaitToTurnOn());
    }

    // Activates the alarm lights after a set of time
    private IEnumerator WaitToTurnOn()
    {
        yield return new WaitForSeconds(2f);
        if (toggle)
        {
            toggle.enabled = true;
        }
    }
}
