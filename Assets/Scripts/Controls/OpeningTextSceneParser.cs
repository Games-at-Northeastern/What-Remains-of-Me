using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Ink.Runtime;
using Ink.UnityIntegration;
using TMPro;
using System;
using UnityEngine.SceneManagement;
public class OpeningTextSceneParser : MonoBehaviour
{
    public TextAsset inkJSON;
    public InkFile inkFile;
    public InputActionReference binding;
    public TMP_Text displayText;


    Story story;


    private void OnEnable()
    {
        if (displayText && binding)
        {
            story = new Story(inkFile.storyJson);
            if (!story.canContinue)
            {
                return;
            }
            binding.action.Enable();
            binding.action.performed += ShowNextLine;
        }
    }

    private void OnDisable()
    {
        binding.action.performed -= ShowNextLine;
    }

    void ShowNextLine(InputAction.CallbackContext context)
    {
        if (story.canContinue)
        {
            displayText.text += "\n\n> " + story.Continue();
        }
        else if (!story.canContinue)
        {
            binding.action.performed -= ShowNextLine;
            binding.action.Disable();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
