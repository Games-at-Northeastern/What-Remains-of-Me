using UnityEngine;

[CreateAssetMenu]
public class DialogueSO : ScriptableObject
{
    //the name of the character to be displayed
    [SerializeField] private string name;

    // an array of strings with each index being a sentence
    // Text area allows us to input from the Unity Scene
    [TextArea(3, 10)]
    [SerializeField] private string[] sentences;

    public string Name => name;

    public string[] Sentences => sentences;
}
