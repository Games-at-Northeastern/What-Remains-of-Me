using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAudioInfo", menuName = "ScriptableObjects/DialogueAudioInfoSO", order = 1)]

public class DialogueAudioInfoSO : ScriptableObject
{
    public string id;
    public AudioClip[] dialogueTypingSoundClips;

    [Range(1, 5)]
    public int clipFrequency;
    [Range (-3, 3)]
    public float minPitch = 0.5f;
    [Range (-3, 3)]
    public float maxPitch = 3f;
    [Range(0, 1f)] public float volume = 1f;
}

