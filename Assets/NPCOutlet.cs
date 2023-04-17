using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCOutlet : AControllable
{
    [SerializeField] private InkDialogueTrigger dialogueTrigger;
    [SerializeField] private TextAsset cleanScript;
    [SerializeField] private TextAsset infectedScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(virus > 20) {
            dialogueTrigger.inkJSON = infectedScript;
        } else {
            dialogueTrigger.inkJSON = cleanScript;
        }
    }
}
