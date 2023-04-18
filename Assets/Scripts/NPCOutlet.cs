using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCOutlet : AControllable
{
    [SerializeField] private InkDialogueTrigger dialogueTrigger;
    [SerializeField] private TextAsset cleanScript;
    [SerializeField] private TextAsset infectedScript;
    [SerializeField] private int virusLevelUpdate;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(virus > virusLevelUpdate) {
            dialogueTrigger.inkJSON = infectedScript;
        } else {
            dialogueTrigger.inkJSON = cleanScript;
        }
    }
}
