using UnityEngine;

public class OrbServerManager : MonoBehaviour
{
    #region terminals
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
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject blueLight;
    [SerializeField] private GameObject pinkLight;

    [SerializeField] private InkDialogueTrigger dialogueTrigger;

    [SerializeField] private NPCOutlet npcOutlet;

    [Header("inkJSONs")]
    public TextAsset noTerminalsInkJSON;
    public TextAsset oneTerminalInkJSON;
    public TextAsset twoTerminalsInkJSON;
    public TextAsset threeTerminalsInkJSON;
    public TextAsset fourTerminalsInkJSON;
    #endregion



    void Update()
    {
        firstTerminalCorrect = FirstTerminalCheck();
        secondTerminalCorrect = SecondTerminalCheck();
        thirdTerminalCorrect = ThirdTerminalCheck();
        fourthTerminalCorrect = FourthTerminalCheck();
        terminalCount = CountCorrect();
        //setText();
    }

    private bool FirstTerminalCheck()
    {
        if (firstTerminal.GetEnergy() >= 50 && firstTerminal.GetEnergy() <= 70)
        {
            if (firstTerminal.GetVirus() >= 15 && firstTerminal.GetVirus() <= 25)
            {
                redLight.SetActive(true);
                return true;
            }
            else
            {
                redLight.SetActive(false);
                return false;
            }
        }
        else
        {
            redLight.SetActive(false);
            return false;
        }
    }
    private bool SecondTerminalCheck()
    {
        if (secondTerminal.GetEnergy() >= 40 && secondTerminal.GetEnergy() <= 60)
        {
            if (secondTerminal.GetVirus() <= 5)
            {
                greenLight.SetActive(true);
                return true;
            }
            else
            {
                greenLight.SetActive(false);
                return false;
            }
        }
        else
        {
            greenLight.SetActive(false);
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
                return true;
            }
            else
            {
                blueLight.SetActive(false);
                return false;
            }
        }
        else
        {
            blueLight.SetActive(false);
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
                return true;
            }
            else
            {
                pinkLight.SetActive(false);
                return false;
            }
        }
        else
        {
            pinkLight.SetActive(false);
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
            dialogueTrigger.inkJSON = noTerminalsInkJSON;
            npcOutlet.SetCleanScript(noTerminalsInkJSON);
            npcOutlet.SetInfectedScript(noTerminalsInkJSON);
        }
        else if (terminalCount == 1)
        {
            dialogueTrigger.inkJSON = oneTerminalInkJSON;
            npcOutlet.SetCleanScript(oneTerminalInkJSON);
            npcOutlet.SetInfectedScript(oneTerminalInkJSON);
        }
        else if (terminalCount == 2)
        {
            dialogueTrigger.inkJSON = twoTerminalsInkJSON;
            npcOutlet.SetCleanScript(twoTerminalsInkJSON);
            npcOutlet.SetInfectedScript(twoTerminalsInkJSON);
        }
        else if (terminalCount == 3)
        {
            dialogueTrigger.inkJSON = threeTerminalsInkJSON;
            npcOutlet.SetCleanScript(threeTerminalsInkJSON);
            npcOutlet.SetInfectedScript(threeTerminalsInkJSON);
        }
        else if (terminalCount == 4)
        {
            dialogueTrigger.inkJSON = fourTerminalsInkJSON;
            npcOutlet.SetCleanScript(fourTerminalsInkJSON);
            npcOutlet.SetInfectedScript(fourTerminalsInkJSON);
        }

        dialogueTrigger.resetVisualCue();
    }
}
