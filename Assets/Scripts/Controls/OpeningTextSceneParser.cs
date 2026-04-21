using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Ink.Runtime;
using TMPro;
using System;
using UnityEngine.SceneManagement;
public class OpeningTextSceneParser : MonoBehaviour
{
    public TextAsset inkJSON;
    public InputActionReference binding;
    public TMP_Text displayText;

    Story story;

    private void OnEnable()
    {
        if (inkJSON && displayText && binding)
        {
            story = new Story(inkJSON.text);
            if (!story.canContinue)
            {
                return;
            }
            binding.action.Enable();
            binding.action.performed += ctx => ShowNextLine();
        }
    }

    private void OnDisable()
    {
        binding.action.performed -= ctx => ShowNextLine();
    }

    void ShowNextLine()
    {
        if (story.canContinue)
        {
            displayText.text += "\n\n>" + story.Continue();
        }
        if (!story.canContinue)
        {
            binding.action.performed -= ctx => ShowNextLine();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
