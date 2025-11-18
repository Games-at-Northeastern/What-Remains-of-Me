using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AlarmSoundTrigger : MonoBehaviour, IAlarmListener
{
    [SerializeField] private KeyOutlet keyOutlet;

    private void Start() => keyOutlet.Subscribe(this);

    public void OnAlarmStart() => GetComponent<AudioSource>().Play();
}