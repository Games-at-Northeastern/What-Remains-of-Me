using UnityEngine;


// Scriptable object used to hold a reference to the runtime player manager
[CreateAssetMenu(fileName = "PlayerRef", menuName = "ScriptableObjects/PlayerRef")]
public class PlayerRef : ScriptableObject
{
    private static PlayerRef runtimeInstance;
    private PlayerManager playerManager;

    // Static reference to the runtime player manager
    // While scriptable object data always persists, the PlayerManager reference
    // becomes null when the program ends
    public static PlayerManager PlayerManager {
        get => Instance.playerManager;
        set {
            // Stores a reference to the player manager if it hasn't already
            if (Instance.playerManager != null) {
                Debug.LogError("PlayerManager is already set!");
                return;
            }

            if (value == null) {
                Debug.LogError("Can't set PlayerManager to null!");
                return;
            }

            Instance.playerManager = value;
        }
    }

    // Reference to the runtime PlayerRef Scriptable Object instance
    public static PlayerRef Instance {
        get {
            // If instance is null
            if (runtimeInstance == null) {
                // Create a copy of the PlayerRef script in the resources folder
                // Make that copy a runtime instance
                PlayerRef scriptableObject = Resources.Load<PlayerRef>(nameof(PlayerRef));
                runtimeInstance = Instantiate(scriptableObject);

                // If the runtime instance is still null, the scriptable object is likely missing
                if (runtimeInstance == null) {
                    Debug.LogError("No PlayerRef found in the Resources folder!");
                }
            }

            return runtimeInstance;
        }
    }
}
