using UnityEngine;

public class TextCutscene : MonoBehaviour
{
    [SerializeField] string dialogCSVFile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    using Ink.Runtime;
using System.Collections.Generic;

List<string> GetAllDialogue(string inkJson)
{
    var story = new Story(inkJson);
    var lines = new List<string>();

    while (story.canContinue)
    {
        string text = story.Continue().Trim();
        if (!string.IsNullOrEmpty(text))
        {
            lines.Add(text);
        }
    }

    return lines;
}
    */
}
