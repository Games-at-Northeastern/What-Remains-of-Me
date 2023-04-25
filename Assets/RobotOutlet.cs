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
    [SerializeField] private int virusLevelUpdate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(energy <= 1) {
            spriteRenderer.sprite = deadRobot;
        } else {
            spriteRenderer.sprite = chargedRobot;
        }
        
        if(energy <= 1) {
            dialogueTrigger.setDialogueActive(false);
        } else if(virus > virusLevelUpdate) {
            dialogueTrigger.inkJSON = infectedScript;
            dialogueTrigger.setDialogueActive(true);

        } else {
            dialogueTrigger.inkJSON = cleanScript;
            dialogueTrigger.setDialogueActive(true);
        }

        
    }
}
