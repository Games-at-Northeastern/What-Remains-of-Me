using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class InkDialogueManager : MonoBehaviour
{
    [Header("Parms")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;

    private Animator layoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset globalsJSON;
    private Story globalsStory;

    private Story currentStory;

    private Coroutine displayLineCoroutine;

    private bool canContinueToNextLine = false;
    public bool dialogueIsPlaying { get; private set; }

    private static InkDialogueManager instance;

    private ControlSchemes _cs;

    // constants for ink tags (ink tags allow you to change the state of the game from ink json files)

    private const string SPEAKER_TAG = "speaker";

    private const string PORTRAIT_TAG = "portrait";

    private const string LAYOUT_TAG = "layout";

    // dialogue variables

    private InkDialogueVariables dialogueVariables;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        dialogueVariables = new InkDialogueVariables(globalsJSON);
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
        //globalsStory = new Story(globalsJSON.text);

        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        // get all of the choices text 
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        // NOTE: The 'currentStory.currentChoiecs.Count == 0' part was to fix a bug after the Youtube video was made
        if (canContinueToNextLine
            && currentStory.currentChoices.Count == 0
            && _cs.Player.Dialogue.WasReleasedThisFrame())
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);
        dialogueVariables.updateDeathCount(currentStory);

        // resets to defaults (makes sure that ink tags don't carry over between npcs)
        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("right");

        ContinueStory();
    }


    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f); // waits a moment to exit dialogue to ensure nothing happens if dialogue key is bound to something else like jump

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            // set text for the current dialogue line
            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));

            // handles current tags
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        // empty dialogue text
        dialogueText.text = "";

        canContinueToNextLine = false;

        //hide previous choices
        HideChoices();

        // display 1 letter at a time
        foreach (char letter in line.ToCharArray())
        {

            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // display choices, if any, for this dialogue line
        DisplayChoices();

        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach (GameObject choicebutton in choices)
        {
            choicebutton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop through current tags
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be parsed:" + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    Debug.Log("Speaker = " + tagValue);
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    Debug.Log("Potrait = " + tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    Debug.Log("Layout = " + tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but isn't being handled" + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                + currentChoices.Count);
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        // Event System requires we clear it first, then wait
        // for at least one frame before we set the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);

            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object result = null;
        dialogueVariables.variables.TryGetValue(variableName, out result);
        if (result == null)
        {
            Debug.LogWarning("Ink Variable was found to be null" + variableName);
        }
        return result;
    }

    public void ChangeVariableState(string variableName, Ink.Runtime.Object newValue)
    {
        Ink.Runtime.Object result = null;
        dialogueVariables.variables.TryGetValue(variableName, out result);
        if (result == null)
        {
            Debug.LogWarning("Ink Variable was found to be null. You may have to add the variable to globas.ink" + variableName);
        }
        else
        {
            dialogueVariables.variables.Remove(variableName);
            dialogueVariables.variables.Add(variableName, newValue);
        }
    }

}
