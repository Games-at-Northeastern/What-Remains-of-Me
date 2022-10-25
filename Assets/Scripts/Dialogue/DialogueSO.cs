using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueSO : ScriptableObject
{
    //the name of the character to be displayed
    public string[] name;

    // an array of strings with each index being a sentence
    // Text area allows us to input from the Unity Scene
    [TextArea(3, 10)]
    public string[] sentences;
}
