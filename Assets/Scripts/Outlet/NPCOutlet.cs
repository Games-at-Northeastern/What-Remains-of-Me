using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.UnityIntegration;

public class NPCOutlet : AControllable
{
    [SerializeField] private InkDialogueTrigger dialogueTrigger;
    //[SerializeField] private TextAsset cleanScript;
    //[SerializeField] private TextAsset infectedScript;
    [SerializeField] private InkFile cleanScript;
    [SerializeField] private InkFile infectedScript;



    [SerializeField] private int virusLevelUpdate;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(virus > virusLevelUpdate) {
            dialogueTrigger.KeyboardInkJSON = infectedScript;
        } else {
            dialogueTrigger.KeyboardInkJSON = cleanScript;
        }
    }

    public void SetCleanScript(InkFile newText) {
        cleanScript = newText;
    }

    public void SetInfectedScript(InkFile newText) {
        infectedScript = newText;
    }

}
