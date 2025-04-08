using UnityEngine;

/**
    This script is to be used with the ActiveScreenManager. It sets the active screen to 
    the triggered one (thru the dialogue trigger) by traversing up to the root of the trigger 
    and then finds the VoxScreenObject. 
*/

public class SetActiveVoxSxcreen : MonoBehaviour
{
    // Triggers when dialogue triggered. 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Traverse to parent object (SmallVoxScreen#)
        Transform root = transform.parent; 

        if (root != null)
        {
            Transform smallVox = root.Find("SmallVox");
            if (smallVox != null)
            {
                Transform voxScreen = smallVox.Find("VoxScreen");
                if (voxScreen != null)
                {
                    // Sets the active screen to the current screen. 
                    ActiveScreenManager.Instance.SetActiveScreen(gameObject);
                    Debug.Log($"Set active VoxScreen to {voxScreen.name}");
                }
                else
                {
                    Debug.LogWarning("VoxScreen not found as a child of SmallVox");
                }
            }
            else 
            {
                Debug.LogWarning("SmallVox not found in root object");
            }
        }
        else 
        {
            Debug.LogWarning("Trigger has no parent to find VoxScreen in");
        }
        // Sets the active screen to the screen player is at. 
        ActiveScreenManager.Instance.SetActiveScreen(gameObject);
    }
}
