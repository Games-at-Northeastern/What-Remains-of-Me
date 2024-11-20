using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeHandler : MonoBehaviour
{
    public static bool DashAllowed { get; private set; }
    public static bool HasVoiceBox { get; private set; }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    /// <summary>
    /// Reset variables for scene load. (Temporary, for debugging)
    /// </summary>
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        DashAllowed = false;
    }

    public static void SetDashAllowed(bool allowed)
    {
        DashAllowed = allowed;
    }

    public static void SetVoiceBox(bool box) => HasVoiceBox = box;
}
