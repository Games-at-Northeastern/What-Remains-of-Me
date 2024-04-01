using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer3D : MonoBehaviour
{
    public Sound sound;
    public AudioSource source;

    private void Start() => SoundController.instance.Add3DSound(this);

    public void Play()
    {
        //if game object is disabled just don't play
        if (!gameObject.activeSelf)
            return;

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
        //if game object is disabled just don't stop
        if (!gameObject.activeSelf)
            return;

        source.Stop();

        return;
    }
}
