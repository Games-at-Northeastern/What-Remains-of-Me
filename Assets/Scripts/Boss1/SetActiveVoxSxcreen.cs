using UnityEngine;

/**
    This script is to be used with the ActiveScreenManager. It sets the active screen to 
    the triggered one (thru the dialogue trigger) by traversing up to the root of the trigger 
    and then finds the VoxScreenObject. 
    *NOTE* to use this script, attach it to the trigger object for dialogue. NOT a screen. 
*/

public class SetActiveVoxSxcreen : MonoBehaviour
{
    // Triggers when dialogue triggered. 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Traverse to parent object (SmallVoxScreen#)
        Transform root = transform.parent; 
        GameObject screen = GetVoxScreenOnTrigger(); 
        if (screen != null)
        {
            // Sets the active screen to the triggered one. 
            ActiveScreenManager.Instance.SetActiveScreen(screen); 
            Debug.Log($"Set active VoxScreen to {root.name}");
        }
        else 
        {
            Debug.LogWarning("Trigger has no parent to find VoxScreen in");
        }
    }

    // Factored out method to get the screen when triggered. 
    private GameObject GetVoxScreenOnTrigger()
    {
        Transform parent = transform.parent;
        if (parent == null)
        {
            Debug.LogWarning("Trigger has no parent.");
            return null;
        }

        // Find small vox within parent. 
        Transform smallVox = parent.Find("SmallVox"); 
        if (smallVox == null)
        {
            Debug.LogWarning($"SmallVox not found under {parent.name}");
            return null;
        }

        // Search for a child w/ an Animator 
        Animator animator = smallVox.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"Animator not found in {parent.name}.");
            return null; 
        }
        // Find VoxScreen inside SmallVox
        Transform voxScreen = smallVox.Find("VoxScreen");
        //if (voxScreen == null)
        //{
            //Debug.LogWarning($"VoxScreen not found inside SmallVox.");
            //return null;
        //}
        // returns the vox screen with the animator parent object. 
        Debug.Log($"Found Vox Screen Animator on {parent.name}.");
        return animator.gameObject;
    }
}
