using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New SoundArray", menuName = "SoundArray")]
public class SoundArray : ScriptableObject
{
    public Sound[] sounds;

    //Looks through the SoundArray sounds for the Sound
    //in the array that has the same soundName as the given string.
    //Plays that sound if it is in the array, throws an error otherwise
    public AudioClip GetSound(string soundname)
    {
        foreach (Sound s in sounds)
        {
            if (s.soundName == soundname)
            {
                return s.clip;
            }
        }
        throw new Exception("No Sound with the name " + soundname + " found in the SoundArray");
    }

}
