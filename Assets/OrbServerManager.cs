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
    }

    private bool FirstTerminalCheck()
    {
        if (firstTerminal.GetEnergy() >= 50 && firstTerminal.GetEnergy() <= 70)
        {
            if (firstTerminal.GetVirus() >= 15 && firstTerminal.GetVirus() <= 25)
            {
                redLight.SetActive(true);
                redTerminal.SetActive(true);
                SetText();
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
                SetText();
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
        if (thirdTerminal.GetEnergy() >= 25 && thirdTerminal.GetEnergy() <= 45)
        {
            if (thirdTerminal.GetVirus() >= 25 && thirdTerminal.GetVirus() <= 45)
            {
                blueLight.SetActive(true);
                blueTerminal.SetActive(true);
                SetText();
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
                SetText();
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
        if (terminalCount == 0)
        {
            currentText = noTerminalsInkJSON;
        }
        else if (terminalCount == 1)
        {
            currentText = oneTerminalInkJSON;
            Speak(oneTerminalHasFired);
        }
        else if (terminalCount == 2)
        {
            currentText = twoTerminalsInkJSON;
            Speak(twoTerminalHasFired);
        }
        else if (terminalCount == 3)
        {
            currentText = threeTerminalsInkJSON;
            Speak(threeTerminalHasFired);
        }
        else if (terminalCount == 4)
        {
            currentText = fourTerminalsInkJSON;
            Speak(fourTerminalHasFired);
        }
        dialogueTrigger.inkJSON = currentText;
        npcOutlet.SetCleanScript(currentText);
        npcOutlet.SetInfectedScript(currentText);
        dialogueTrigger.ResetVisualCue();
    }




    //####################################### Ambient Dialogue ################################### 
    

    public void Speak(bool hasFired) {
        if(!hasFired){
        var i = InkDialogueManager.GetInstance();
            i.waitBeforePageTurn = 2f;
            i.stopMovement = false;
            i.autoTurnPage = true;
            i.EnterDialogueMode(currentText);
        }
        hasFired = true;
    }
}
