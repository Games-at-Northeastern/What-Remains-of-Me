using UnityEngine;

/**
    Singleton class that manages the instances of the small vox screens. 
    This is to prevent the wrong screen from triggering its animation. 
    *NOTE* this script should be attached to 1 object within a scene, not multiple. 
*/
public class ActiveScreenManager : MonoBehaviour
{
    public static ActiveScreenManager Instance { get; private set;}

    // Active screen object reference 
    public GameObject activeScreen; 

    private void Awake()
    {
        // Destroy if an instance already exists 
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this; 
    }

    // Set active screen 
    public void SetActiveScreen(GameObject screen) 
    {
        activeScreen = screen; 
        Debug.Log($"Active screen has been set");
    }

    // Get active screen
    public GameObject GetActiveScreen() 
    {
        return activeScreen;
    }

    // Resets active scene. 
    public void ClearActiveScreen()
    {
        activeScreen = null; 
    }
   
}
