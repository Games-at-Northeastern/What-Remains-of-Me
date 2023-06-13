using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotOutlet : AControllable
{
    [SerializeField] private InkDialogueTrigger dialogueTrigger;
    [SerializeField] private TextAsset cleanScript;
    [SerializeField] private TextAsset infectedScript;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite deadRobot;
    [SerializeField] private Sprite chargedRobot;
    [SerializeField] private Sprite virusRobot;
    [SerializeField] private int virusLevelUpdate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // this if is an intial question to check energy state of the robot, and see if they are active. If you wanted more states 
       // of energy, bring the virus switch case into a helper function, and replace the if statement with a switch statement for energy.
       // You can bring the virus switch case into a hepler function, and call this from the energy switch case. The idea is you have
       // different states of energy, changing how the robot reacts. Then on top of the energy, there is a virus switch case checking how
       // behavior should change depending on virus
         
       if(cleanEnergy > 1 || virus > 1) {
        spriteRenderer.sprite = chargedRobot;
        
        switch (virus) 
        {
            case float virus when (virus < 50):
            dialogueTrigger.inkJSON = cleanScript;
            dialogueTrigger.setDialogueActive(true);
            break;

            //case float v when (v >= 25):
            // these extra cases can be added in as more nuanced behavior is added into the game. Currently we are just working with 
            // three states, no power, power but not infected, power but infected

            //case float v when (v >= 75):

            case float virus when (virus >= 50):
            dialogueTrigger.inkJSON = infectedScript;
            dialogueTrigger.setDialogueActive(true);
            spriteRenderer.sprite = virusRobot;

            break;

        }

       } else {
            dialogueTrigger.setDialogueActive(false);
            spriteRenderer.sprite = deadRobot;
       }
        
    }
}
