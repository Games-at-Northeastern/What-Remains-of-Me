using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New SoundArray", menuName = "SoundArray")]
public class SoundArray : ScriptableObject
{
    public Sound[] sounds;

    private AudioClip clipToReturn;

    // Finds the Sound ScriptableObject in the array sounds that has the same soundName as the given string.
    // If it finds a Sound in the array sounds that has the same soundName as the given string, it returns the clip of the Sound.
    // Throws an error if it doesn't find a Sound in the array sounds that has the same soundName as the given string.
    public AudioClip GetSound(string soundname)
    {
        clipToReturn = Array.Find<Sound>(sounds, sound => sound.soundName == soundname).clip;

        if(clipToReturn != null)
        {
            return clipToReturn;
        }
        else
        {
            throw new Exception("No Sound with the name " + soundname + " found in the array sounds of SoundArray");
        }
    }

}
