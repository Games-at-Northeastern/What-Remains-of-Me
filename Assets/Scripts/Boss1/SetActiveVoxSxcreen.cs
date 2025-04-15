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
        GameObject screen = GetVoxScreenOnTrigger(); 
        if (screen != null)
        {
            // Sets the active screen to the triggered one. 
            ActiveScreenManager.Instance.SetActiveScreen(screen); 
            Debug.Log($"Set active VoxScreen to {screen.name}");
        }
        else 
        {
            Debug.LogWarning("Trigger has no parent to find VoxScreen in");
        }
    }

    // Factored out method to get the screen when triggered. 
    private GameObject GetVoxScreenOnTrigger()
    {
        // Traverse to parent object (SmallVoxScreen#)
        Transform root = transform.parent; 
        if (root == null)
        {
            Debug.LogWarning("Trigger has no parent to find VoxScreen in");
            return null; 
        }

        // Traverse for animator object parent (SmallVox)
        Transform smallVox = root.Find("SmallVox"); 
        if (smallVox == null)
        {
            Debug.LogWarning("SmallVox not found in root object");
            return null; 
        }

        // Traverse for animator controller object (VoxScreen)
        Transform voxScreen = smallVox.Find("VoxScreen"); 
        if (voxScreen ==  null) {
            Debug.LogWarning("VoxScreen not found as a child of SmallVox");
            return null; 
        }
        return voxScreen?.gameObject; 
    }
}
