using UnityEngine;
using System.Collections;

public class AlarmParticleEffectTrigger : MonoBehaviour, IAlarmListener
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private TimedToggle particleToggle;
    [SerializeField] private KeyOutlet keyOutlet;
    [SerializeField] private bool isToggle = true;

    void Start()
    {

        keyOutlet.Subscribe(this);
        if (particles)
            particles.gameObject.SetActive(false);
    }

    public void OnAlarmStart()
    {

        StartCoroutine(WaitToTurnOn());
    }

    // Activates the particle effect's game object after a set of time
    private IEnumerator WaitToTurnOn()
    {
        yield return new WaitForSeconds(2f);
        if (particleToggle && isToggle)
            particleToggle.enabled = true;
        else if (particles)
            particles.gameObject.SetActive(true);
    }
}
