using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;

public class InkDialogueManager : MonoBehaviour
{

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    private ControlSchemes _cs;
    private static InkDialogueManager instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one ink dialogue manager in the scene");
        }
        instance = this;
    }

    public static InkDialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        _cs = new ControlSchemes();
        _cs.Enable();

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (_cs.Player.Dialogue.triggered)
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();

    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }



}
