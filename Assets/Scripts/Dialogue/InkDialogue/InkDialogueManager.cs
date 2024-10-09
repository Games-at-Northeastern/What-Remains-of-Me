using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using PlayerController;

public class InkDialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float dialogueDelayTime = 100f;
    [SerializeField] private float exitDialogueTime = 1.0f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanelRight;
    [SerializeField] private GameObject dialoguePanelTop;
    [SerializeField] private TextMeshProUGUI dialogueTextRight;
    [SerializeField] private TextMeshProUGUI dialogueTextTop;
    [SerializeField] private TextMeshProUGUI displayNameTextRight;
    [SerializeField] private TextMeshProUGUI displayNameTextTop;
    [SerializeField] private Animator portraitAnimatorRight;
    [SerializeField] private Animator portraitAnimatorTop;

    [Header("Audio")]
    [SerializeField] private AudioClip dialogueTypingSoundClip;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 3f;
    [Range(-3, 3)]
    private static AudioSource audioSource;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset globalsJSON;

    [Header("Configs")]
    public bool stopMovement;
    public bool autoTurnPage;
    public float waitBeforePageTurn;
    public bool dialogueEnded;

    public bool dialogueIsPlaying { get; private set; }


    public PlayerController2D cc;

    // constants for ink tags (ink tags allow you to change the state of the game from ink json files)
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    // dialogue variables
    private InkDialogueVariables dialogueVariables;

    // private variables
    private Story currentStory;
    private Coroutine displayLineCoroutine;
    private bool canContinueToNextLine = false;
    private Animator layoutAnimator;
    private static InkDialogueManager instance;
    private ControlSchemes _cs;
    private bool canSkip = false;
    private bool submitSkip = false;
    private TextMeshProUGUI dialogueText;

    [HideInInspector]
    public bool isTutorialDialogue = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        dialogueVariables = new InkDialogueVariables(globalsJSON);

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public static InkDialogueManager GetInstance()
    {
        if (instance == null)
        {
            instance = new InkDialogueManager();
        }
        return instance;
    }

    private void Start()
    {
        _cs = new ControlSchemes();
        _cs.Enable();
        dialogueIsPlaying = false;
        dialogueText = dialogueTextRight;
        dialoguePanelRight.SetActive(false);
        dialoguePanelTop.SetActive(false);
        stopMovement = true;
        dialogueEnded = false;

        layoutAnimator = dialoguePanelRight.GetComponent<Animator>();

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

        if (_cs.Player.Dialogue.WasPressedThisFrame())
        {
            submitSkip = true;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        // NOTE: The 'currentStory.currentChoiecs.Count == 0' part was to fix a bug after the Youtube video was made
        if (canContinueToNextLine
            && currentStory.currentChoices.Count == 0)
             {
            if((_cs.Player.Dialogue.WasPressedThisFrame() && dialogueText == dialogueTextRight) || autoTurnPage)
        {
            ContinueStory();
        }
        else if(dialogueText == dialogueTextTop && dialogueText.maxVisibleCharacters == dialogueText.text.Length) {

            ContinueStory();
        }
    }
    }

    private IEnumerator ContinueWithDelay()
    {
        yield return new WaitForSeconds(dialogueDelayTime);
        ContinueStory();
    }

    public void EnterDialogueMode(TextAsset inkJSON) 
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        
        // This now requires a character controller for the player to be placed into the dialogue manager in every level.
        if (stopMovement)
        {
            dialoguePanelRight.SetActive(true);
            cc.LockInputs();
        } else {
            dialoguePanelTop.SetActive(true);
        }

        dialogueVariables.StartListening(currentStory);
        dialogueVariables.updateInkVarsThatChangeInGame(currentStory);

        // resets to defaults (makes sure that ink tags don't carry over between npcs)
        displayNameTextRight.text = "???";
        portraitAnimatorRight.Play("default");
        layoutAnimator.Play("right");

        ContinueStory();

    }


    private IEnumerator ExitDialogueMode()
    {
        if (isTutorialDialogue)
        {
            yield return new WaitForSeconds(exitDialogueTime); // waits a moment to exit dialogue to ensure nothing happens if dialogue key is bound to something else like jump
        }
        else
        {
            yield return new WaitForSeconds(0f); // waits a moment to exit dialogue to ensure nothing happens if dialogue key is bound to something else like jump
        }

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialogueEnded = true;
        //turns off the X constraint on the player's rigidbody when dialogue has stopped
        if (stopMovement)
        {
            dialoguePanelRight.SetActive(false);
            cc.UnlockInputs();
        } else {
            dialoguePanelTop.SetActive(false);
        }
        //btw if you ever want to adjust this, know that RigidbodyContraints2D are something called a "Bitmap" so they can't be set normally
        // https://answers.unity.com/questions/1104653/im-trying-to-freeze-both-positionx-and-rotation-in.html 
        // The constraints property is a Bitmask. Simply setting it to a single option only sets that option. You need to use the | (bitwise OR) operator to merge them together before setting it i.e.
        // constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezepositionX;
       
        dialogueText.text = "";
}

private void ContinueStory()
{
StopCoroutine(ContinueWithDelay());
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
    if(stopMovement) {
        dialogueText = dialogueTextRight;
    } else {
        dialogueText = dialogueTextTop;
    }
    
// empty dialogue text
dialogueText.text = line;
dialogueText.maxVisibleCharacters = 0;

canContinueToNextLine = false;
submitSkip = false;

StartCoroutine(CanSkip());

//hide previous choices
HideChoices();

// display 1 letter at a time
foreach (char letter in line.ToCharArray())
{
    // if player presses 'submit', entire dialogue line is displayed
    if (canSkip && submitSkip && dialogueText == dialogueTextRight)
    {
        submitSkip = false;
        dialogueText.maxVisibleCharacters = line.Length;
        break;
    }
    dialogueText.maxVisibleCharacters++;
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(dialogueTypingSoundClip);
    yield return new WaitForSeconds(typingSpeed);
}


// display choices, if any, for this dialogue line
DisplayChoices();

if (canSkip)
{
    StartCoroutine(ContinueWithDelay());
} else if (autoTurnPage)
{
    yield return new WaitForSeconds(waitBeforePageTurn);
}
if(dialogueText == dialogueTextRight) {
canContinueToNextLine = true;
}
else 
{
yield return new WaitForSeconds(2.0f);
canContinueToNextLine = true;
}
canSkip = false;
}

private IEnumerator CanSkip()
{
canSkip = false; 
yield return new WaitForSeconds(0.05f);
canSkip = true;
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
        //This can definetly get coded better. But the method i was using kept getting a NullRefrenceException error so I reverted to this way. 
        case SPEAKER_TAG:
        if(stopMovement) {
            displayNameTextRight.text = tagValue;
            Debug.Log("Speaker = " + tagValue);
        } else {
            displayNameTextTop.text = tagValue;
            Debug.Log("Speaker = " + tagValue);
        }
            break;
        case PORTRAIT_TAG:
        if(stopMovement) {
            portraitAnimatorRight.Play(tagValue);
            Debug.Log("Potrait = " + tagValue);
        } else {
            portraitAnimatorTop.Play(tagValue);
            Debug.Log("Potrait = " + tagValue);  
        }
            break;
        case LAYOUT_TAG:
        if(stopMovement) {
            layoutAnimator = dialoguePanelRight.GetComponent<Animator>();
            layoutAnimator.Play(tagValue);
            Debug.Log("Layout = " + tagValue);
        } else {
            layoutAnimator = dialoguePanelTop.GetComponent<Animator>();
            layoutAnimator.Play(tagValue);
            Debug.Log("Layout = " + tagValue);

        }
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

// this method will allow for a variable defined in globals.ink to be set using C# code
public void SetVariableState(string variableName, Ink.Runtime.Object variableValue) 
{
        if (dialogueVariables.variables.ContainsKey(variableName)) 
        {
            dialogueVariables.variables.Remove(variableName);
            dialogueVariables.variables.Add(variableName, variableValue);
        }
        else 
        {
            Debug.LogWarning("Tried to update variable that wasn't initialized by globals.ink: " + variableName);
        }
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
