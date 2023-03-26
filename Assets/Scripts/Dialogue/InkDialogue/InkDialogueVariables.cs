using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkDialogueVariables
{
    // dictionary where vars are stored
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    // constructor
    public InkDialogueVariables(TextAsset textAsset)
    {
        Story globalVars = new Story(textAsset.text);

        // initializes dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVars.variablesState)
        {
            Ink.Runtime.Object value = globalVars.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialouge variable: " + name + "=" + value);
        }
    }


    // maintains variables that are initialized in the dictionary
    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

    public void StartListening(Story story)
    {
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }
}
