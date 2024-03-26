using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer3D : MonoBehaviour
{
    public Sound sound;
    [SerializeField] AudioSource source;

    private void Start() => SoundController.instance.Add3DSound(this);

    public void Play()
    {
        //Debug.Log("sound played " + name + " source : " + s.source + ", " + s.source.gameObject.name);
        source.clip = sound.clip;
        source.pitch = sound.basePitch + Random.Range(sound.lowerPitchRandomizer, sound.higherPitchRandomizer);
        source.volume = sound.baseVolume;
        source.loop = sound.loop;
        source.Play();

        return;
    }

    public void Stop()
    {
        source.Stop();

        return;
    }
}
