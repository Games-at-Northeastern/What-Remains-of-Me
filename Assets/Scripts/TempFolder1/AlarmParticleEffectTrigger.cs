using UnityEngine;
using System.Collections;

public class AlarmParticleEffectTrigger : MonoBehaviour, IAlarmListener
{
    [SerializeField] private GameObject particleEffect;
    [SerializeField] private KeyOutlet keyOutlet;

    void Start()
    {
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
        Instantiate(particleEffect, transform.position, transform.rotation);
    }
}
