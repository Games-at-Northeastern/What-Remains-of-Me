using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Fires an event if this object is touched by the player. Destroys this object once
/// collected.
/// </summary>
public class EventCollectable : MonoBehaviour
{
    public UnityEvent onCollected;
    public PlayerInfo playerInfo;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            onCollected.Invoke();
            Destroy(gameObject);
            //test for setting the global voice command ink var.
            //this will need to be a more generic robust system that can set arbitrary
            //global ink vars not just this hard coded one. Also will need to add persistance
            //saving the globals to a file.
            // convert the variable into a Ink.Runtime.Object value
            bool voiceModuleObtained = true;
            Ink.Runtime.Object obj = new Ink.Runtime.BoolValue(voiceModuleObtained);
            // call the DialogueManager to set the variable in the globals dictionary
            InkDialogueManager.GetInstance().SetVariableState("voiceModuleObtained", obj);
            playerInfo.GainModule(UpgradeType.VOICEMODULE);
        }
    }
}
