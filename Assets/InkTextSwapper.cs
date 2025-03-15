using UnityEngine;

public class InkTextSwapper : MonoBehaviour
{

    public InkDialogueTrigger inkDialogueTrigger;
    public NPCOutlet npcOutlet;

    public TextAsset newText;
   
   public void SwapText() {
    Debug.Log("Swapped text!");
    inkDialogueTrigger.ResetVisualCue();
    inkDialogueTrigger.inkJSON = newText;
    npcOutlet.SetCleanScript(newText);
    npcOutlet.SetInfectedScript(newText);
   }
}
