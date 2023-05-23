using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://vintay.medium.com/creating-a-respawn-checkpoint-system-in-unity-13e51faf44f3

public class CheckpointManager : MonoBehaviour
{
    // Singleton
    public static CheckpointManager Instance { get; private set; }

    /// <summary>
    /// Since the CheckpointManager is a singleton, makes sure to keep the same instance
    /// of this game object
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
