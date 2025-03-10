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
    #endregion

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
        if (firstTerminal.GetEnergy() >= 75 && firstTerminal.GetEnergy() <= 85)
        {
            if (firstTerminal.GetVirus() >= 15 && firstTerminal.GetVirus() <= 25)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private bool SecondTerminalCheck()
    {
        if (secondTerminal.GetEnergy() >= 45 && secondTerminal.GetEnergy() <= 55)
        {
            if (secondTerminal.GetVirus() <= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private bool ThirdTerminalCheck()
    {
        if (thirdTerminal.GetEnergy() >= 61 && thirdTerminal.GetEnergy() <= 71)
        {
            if (thirdTerminal.GetVirus() >= 28 && thirdTerminal.GetVirus() <= 38)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private bool FourthTerminalCheck()
    {
        if (fourthTerminal.GetEnergy() >= 15 && fourthTerminal.GetEnergy() <= 25)
        {
            if (fourthTerminal.GetVirus() >= 15 && fourthTerminal.GetVirus() <= 25)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
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
        else
            count = 0;
        return count;
    }
}
