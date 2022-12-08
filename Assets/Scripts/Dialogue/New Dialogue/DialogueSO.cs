using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[CreateAssetMenu]
public class DialogueSO : ScriptableObject
{
    [Tooltip("The name of the text file that this object will be stored in. Do not write file-type extension.")]
    [SerializeField] private string filename;

    //the name of the character to be displayed
    [SerializeField] private string name;

    // an array of strings with each index being a sentence
    // Text area allows us to input from the Unity Scene
    [TextArea(3, 10)]
    [SerializeField] private string[] sentences;

    public string Name => GetName();

    public string[] Sentences => GetSentences();


    /// <summary>
    /// reads the sentences for this dialogue object from the specified json text file
    /// </summary>
    /// <returns>the sentences as an array of strings</returns>
    private string[] GetSentences()
    {
        this.LoadFromFile(this.filename + ".txt", out var result);
        var jsonDialogue = new JsonDialogue();
        JsonUtility.FromJsonOverwrite(result, jsonDialogue);
        return jsonDialogue.Sentences;
    }

    /// <summary>
    /// reads the name for this dialogue object from the specified json text file
    /// </summary>
    /// <returns>the name as a string</returns>
    private string GetName()
    {
        this.LoadFromFile(this.filename + ".txt", out var result);
        var jsonDialogue = new JsonDialogue();
        JsonUtility.FromJsonOverwrite(result, jsonDialogue);
        return jsonDialogue.Name;
    }

    /// <summary>
    /// Loads the JSON text file to a result string
    /// </summary>
    /// <param name="fileName">The name of the file to be loaded</param>
    /// <param name="result">the string to load the json to</param>
    /// <returns>true if the file is successfully loaded, false if it needs to be created before loading</returns>
    private bool LoadFromFile(string fileName, out string result)
    {
        var fullPath = "Assets/Scripts/Dialogue/DialogueJsons/" + fileName;

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to read from {fullPath} with exception {e}");
            File.WriteAllText(fullPath, JsonUtility.ToJson(new JsonDialogue(this.name, this.sentences)));
            result = File.ReadAllText(fullPath);
            return false;
        }
    }

    /// <summary>
    /// Serializable class to store the information of a Dialogue Scriptable Object in a text file in a JSON format.
    /// </summary>
    [Serializable]
    private class JsonDialogue
    {
        public string Name;
        public string[] Sentences;

        public JsonDialogue()
        {
            Name = "";
            Sentences = new string[] { };
        }

        public JsonDialogue(string name, string[] sentences)
        {
            Name = name;
            Sentences = sentences;
        }
    }
}
