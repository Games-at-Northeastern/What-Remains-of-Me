using UnityEngine;

/**
    Singleton class that manages the instances of the small vox screens. 
    This is to prevent the wrong screen from triggering its animation. 
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
        Debug.Log($"Active screen has been set to: {screen.name}");
    }

    public GameObject GetActiveScreen() 
    {
        return activeScreen;
    }

    public void ClearActiveScreen()
    {
        activeScreen = null; 
    }
   
}
