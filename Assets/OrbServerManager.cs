using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbServerManager : MonoBehaviour
{
    #region terminals
    [Header("Terminals")]
    [SerializeField] private OrbOutlets firstTerminal;
    [SerializeField] private OrbOutlets secondTerminal;
    [SerializeField] private OrbOutlets thirdTerminal;
    [SerializeField] private OrbOutlets fourthTerminal;
    [SerializeField] private bool firstTerminalCorrect = false;
    [SerializeField] private bool secondTerminalCorrect = false;
    [SerializeField] private bool thirdTerminalCorrect = false;
    [SerializeField] private bool fourthTerminalCorrect = false;
    [SerializeField] public int terminalCount = 0;
    #endregion
    #region lights
    [Header("Lights")]
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject tealLight;
    [SerializeField] private GameObject blueLight;
    [SerializeField] private GameObject pinkLight;
    [SerializeField] private GameObject redTerminal;
    [SerializeField] private GameObject tealTerminal;
    [SerializeField] private GameObject blueTerminal;
    [SerializeField] private GameObject pinkTerminal;


    #endregion
    [Header("Dialogue")]
    [SerializeField] private InkDialogueTrigger dialogueTrigger;

    [SerializeField] private NPCOutlet npcOutlet;
    [SerializeField] private OrbServerCompletion beat;

    public bool zeroTerminalHasFired = false;
    public bool oneTerminalHasFired = false;
    public bool twoTerminalHasFired = false;
    public bool threeTerminalHasFired = false;
    public bool fourTerminalHasFired = false;

    [Header("inkJSONs")]
    public TextAsset noTerminalsInkJSON;
    public TextAsset oneTerminalInkJSON;
    public TextAsset twoTerminalsInkJSON;
    public TextAsset threeTerminalsInkJSON;
    public TextAsset fourTerminalsInkJSON;
    public TextAsset currentText;



    void Update()
    {
        firstTerminalCorrect = FirstTerminalCheck();
        secondTerminalCorrect = SecondTerminalCheck();
        thirdTerminalCorrect = ThirdTerminalCheck();
        fourthTerminalCorrect = FourthTerminalCheck();
        terminalCount = CountCorrect();
        SetText();
    }

    private bool FirstTerminalCheck()
    {
        if (firstTerminal.GetEnergy() >= 40 && firstTerminal.GetEnergy() <= 60)
        {
            if (firstTerminal.GetVirus() >= 15 && firstTerminal.GetVirus() <= 35)
            {
                redLight.SetActive(true);
                redTerminal.SetActive(true);
                if(!oneTerminalHasFired) {
                    StartCoroutine(SpeakDelay());
                    oneTerminalHasFired = true;
                }
                return true;
            }
            else
            {
                redLight.SetActive(false);
                redTerminal.SetActive(false);
                return false;
            }
        }
        else
        {
            redLight.SetActive(false);
            redTerminal.SetActive(false);
            return false;
        }
    }
    private bool SecondTerminalCheck()
    {
        if (secondTerminal.GetEnergy() >= 40 && secondTerminal.GetEnergy() <= 60)
        {
            if (secondTerminal.GetVirus() <= 5)
            {
                tealLight.SetActive(true);
                tealTerminal.SetActive(true);
                if(!twoTerminalHasFired) {
                    StartCoroutine(SpeakDelay());
                    twoTerminalHasFired = true;
                }
                return true;
            }
            else
            {
                tealLight.SetActive(false);
                tealTerminal.SetActive(false);
                return false;
            }
        }
        else
        {
            tealLight.SetActive(false);
            tealTerminal.SetActive(false);
            return false;
        }
    }
    private bool ThirdTerminalCheck()
    {
        if (thirdTerminal.GetEnergy() >= 23 && thirdTerminal.GetEnergy() <= 43)
        {
            if (thirdTerminal.GetVirus() >= 23 && thirdTerminal.GetVirus() <= 43)
            {
                blueLight.SetActive(true);
                blueTerminal.SetActive(true);

                if(!threeTerminalHasFired) {
                    StartCoroutine(SpeakDelay());
                    threeTerminalHasFired = true;
                }
                return true;
            }
            else
            {
                blueLight.SetActive(false);
                blueTerminal.SetActive(false);
                return false;
            }
        }
        else
        {
            blueLight.SetActive(false);
            blueTerminal.SetActive(false);
            return false;
        }
    }
    private bool FourthTerminalCheck()
    {
        if (fourthTerminal.GetEnergy() <= 5)
        {
            if (fourthTerminal.GetVirus() >= 15 && fourthTerminal.GetVirus() <= 30)
            {
                pinkLight.SetActive(true);
                pinkTerminal.SetActive(true);
                if(!fourTerminalHasFired) {
                    StartCoroutine(SpeakDelay());
                    fourTerminalHasFired = true;
                }
                return true;
            }
            else
            {
                pinkLight.SetActive(false);
                pinkTerminal.SetActive(false);
                return false;
            }
        }
        else
        {
            pinkLight.SetActive(false);
            pinkTerminal.SetActive(false);
            return false;
        }
    }
    private int CountCorrect()
    {
        int count = 0;
        if (firstTerminalCorrect)
        {
            count++;
        }
        if (secondTerminalCorrect)
        {
            count++;
        }
        if (thirdTerminalCorrect)
        {
            count++;
        }
        if (fourthTerminalCorrect)
        {
            count++;
        }
        return count;
    }

    public void SetText()
    {
       if (terminalCount == 1)
        {
            currentText = oneTerminalInkJSON;
        }
        else if (terminalCount == 2)
        {
            currentText = twoTerminalsInkJSON;
        }
        else if (terminalCount == 3)
        {
            currentText = threeTerminalsInkJSON;
        }
        else if (terminalCount == 4)
        {
            currentText = fourTerminalsInkJSON;
            beat.isBossBeaten = true;
        }
        dialogueTrigger.KeyboardInkJSON = currentText;
        npcOutlet.SetCleanScript(currentText);
        npcOutlet.SetInfectedScript(currentText);
        dialogueTrigger.ResetVisualCue();
    }




    //####################################### Ambient Dialogue ################################### 
    
    private IEnumerator SpeakDelay() {
        yield return new WaitForSeconds(0.5f);
        Speak();
    }

    public void Speak() {
        var i = InkDialogueManager.GetInstance();
            i.waitBeforePageTurn = 2f;
            i.stopMovement = false;
            i.autoTurnPage = true;
            i.EnterDialogueMode(currentText);
        
    }
}
