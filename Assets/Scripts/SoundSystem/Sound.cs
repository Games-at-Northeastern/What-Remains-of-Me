using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a single sound effect, a song, or a layer in a layered song/sound. Used in tandem with the SoundController.cs
//class. 
[System.Serializable]
public class Sound
{
    public string name; // The name of the sound to be called in any of the SoundController functions.

    public AudioClip clip; // The clip itself (.wav, .mp3, etc.) 

    [Range(0, 1.5f)]
    public float baseVolume = 1; // Base volume of the clip (can be lowered by fade in/out)
    [Range(0.2f, 3f)]
    public float pitch = 1;

    public bool loop; // Should this sound loop itself? 

    [HideInInspector]
    public AudioSource source; // AudioSource associated with this sound.
}
