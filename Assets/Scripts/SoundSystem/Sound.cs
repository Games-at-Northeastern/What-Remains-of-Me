using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//Represents a single sound effect, a song, or a layer in a layered song/sound. Used in tandem with the SoundController.cs
//class. 
[CreateAssetMenu(fileName = "New Sound", menuName = "Sound Effects")]
public class Sound : ScriptableObject
{
    public string soundName; // The name of the sound to be called in any of the SoundController functions.

    public AudioClip clip; // The clip itself (.wav, .mp3, etc.) 

    [Range(0, 1.5f)]
    public float baseVolume = 1; // Base volume of the clip (can be lowered by fade in/out, but this value will not change.)
    [Range(0.2f, 3f)]
    public float basePitch = 1;

    [Range(-1f, 1f)]
    public float lowerPitchRandomizer, higherPitchRandomizer = 0f; // The bounds of what to randomly add to the pitch

    public bool loop; // Should this sound loop itself?

    public AudioMixerGroup audioMixerGroup;

    [HideInInspector]
    public AudioSource source; // AudioSource associated with this sound.
}
