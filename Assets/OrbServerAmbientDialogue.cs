using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbServerAmbientDialogue : MonoBehaviour 
    

{
    public OrbServerManager orbServerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }





    public void StartZeroNodeTimer() {
        StartCoroutine(ZeroNodeDelay());
    }

    private IEnumerator ZeroNodeDelay() {
        if (!orbServerManager.zeroTerminalHasFired) {
        yield return new WaitForSeconds(20);
        Speak();
        orbServerManager.zeroTerminalHasFired = true;
        }
    }

    public void Speak() {
        var i = InkDialogueManager.GetInstance();
            i.waitBeforePageTurn = 2f;
            i.stopMovement = false;
            i.autoTurnPage = true;
            i.EnterDialogueMode(orbServerManager.noTerminalsInkJSON);
    }


}
