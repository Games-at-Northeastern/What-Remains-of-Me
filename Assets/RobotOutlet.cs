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
        if(energy <= 0) {
            spriteRenderer.sprite = deadRobot;
        } else {
            spriteRenderer.sprite = chargedRobot;
        }
        
        if(virus > virusLevelUpdate) {
            dialogueTrigger.inkJSON = infectedScript;
        } else {
            dialogueTrigger.inkJSON = cleanScript;
        }
    }
}
