using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using System;

/// <summary>
/// This class provides the functionalities for the LevelManager singleton,
/// which is used to manage the scene and warp the player between scenes.
/// </summary>
public class LevelManager : MonoBehaviour
{
    private static SerializableStringIntDict immediateTags = new();
    private static SerializableStringIntDict persistentTags = new();
    //private static TagContainer consumeTags = new();
    //private static TagContainer consumeSingleTags = new();
    //private static TagContainer consumeUniqueTags = new();
    //private static TagContainer persistentTags = new();

    public static void SendTagImmediate(string tag, int count) => SendTags(tag, count, immediateTags);
    public static void SendTagPersistent(string tag, int count) => SendTags(tag, count, persistentTags);

    private static void SendTags(string tag, int count, SerializableStringIntDict reciever)
    {
        if (count < 1)
        {
            Debug.LogError("attempting to send tag count < 1");
        }

        if (reciever.ContainsKey(tag))
        {
            reciever[tag] += count;
            return;
        }

        reciever.Add(tag, count);
    }

    //set this on awake, apply according modifications on start
    public static SerializableStringIntDict Tags = new();

    /*private static TagContainer GetTagList(Tags.SendType sendType) => sendType switch
    {
        Tags.SendType.IMMEDIATE => immediateTags,
        Tags.SendType.CONSUME => consumeTags,
        Tags.SendType.CONSUME_SINGLE => consumeSingleTags,
        Tags.SendType.CONSUME_UNIQUE => consumeUniqueTags,
        Tags.SendType.PERSISTENT => persistentTags,
        _ => null,
    };*/

    [SerializeField] private GameObject player;
    private string warpDestination;
    private int toWarpID;
    private bool loadedNewScene = false;
    private static Vector2 recentCheckpointHolder;
    private static bool checkpointHeld;
    public static GameObject PlayerRef { get; private set; }

    // Level events
    [Header("Event to trigger player respawn after obstacle collision.")]
    public static UnityEvent OnPlayerReset = new UnityEvent();
    [Header("Event to trigger player death events.")]
    public static UnityEvent OnPlayerDeath = new UnityEvent();
    public static CheckpointManager checkpointManager;

    // Event start functions that are accessible for other objects to trigger events
    /// <summary>
    /// Event to trigger player respawn after obstacle collision. This should respawn the player at the nearest checkpoint.
    /// </summary>
    public static void PlayerReset() => OnPlayerReset?.Invoke();
    /// <summary>
    /// Event to trigger full player death event. This should respawn the player at the beginning of the level.
    /// </summary>
    public static void PlayerDeath() => OnPlayerDeath?.Invoke(); // TODO: We may want to take in a type of death for this function

    private void Awake()
    {
        OnPlayerReset.RemoveAllListeners();
        OnPlayerDeath.RemoveAllListeners();

        Tags.Clear();
        foreach (var (tag, val) in immediateTags)
        {
            Tags.Add(tag, val);
        }
        immediateTags.Clear();

        foreach (var (tag, val) in persistentTags)
        {
            Tags.Add(tag, val);
        }
    }
    private void Start()
    {
        PlayerRef = FindObjectOfType<PlayerController.PlayerController2D>().gameObject;
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    /// <summary>
    /// If there is no player reference, sets the player reference to the player game object.
    /// If the player is supposed to load into a new scene, loads the player game object
    /// into the scene at the scene's set load in position.
    /// If the player presses the "R" key, resets the player at the tutorial scene.
    /// </summary>
    private void Update()
    {
        if (loadedNewScene)
        {
            GameObject[] warpZones = GameObject.FindGameObjectsWithTag("WarpZone");
            foreach (GameObject warpZone in warpZones)
            {
                LevelWarpZone wz = warpZone.GetComponent<LevelWarpZone>();
                if (wz.id == toWarpID)
                {
                    PlayerRef.transform.position = wz.loadInPosition.position;
                    loadedNewScene = false;
                    return;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket)) {
            CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
            recentCheckpointHolder = checkpointManager.getMostRecentPoint().getRespawnPosition();
            checkpointHeld = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /* Commented out cause prevents game from pausing
         * if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }*/
    }

    /// <summary>
    /// Sets the warp destination.
    /// <param name="warpDestination">the string name of the scene to warp to</param>
    /// <param name="id">an integer id for the scene to warp to</param>
    /// </summary>
    public void SetWarpID(string warpDestination, int id)
    {
        this.warpDestination = warpDestination;
        this.toWarpID = id;
    }

    /// <summary>
    /// Warps to the set warp destination.
    /// </summary>
    public void WarpToScene()
    {
        SceneManager.LoadScene(warpDestination);
        loadedNewScene = true;
    }

    /// <summary>
    /// Returns the checkpoint being held by this LevelManager and stops holding it. In theory, since the LevelManager is persistent, this method should only
    /// need to be called when we have reloaded a level mid-progress and wish to preserve the player's physical progress through the level.
    /// </summary>
    public static Vector2 extractRecentCheckpoint() {
        checkpointHeld = false;
        return recentCheckpointHolder;
    }

    public static bool holdingCheckpoint() {
        return checkpointHeld;
    }
}

#pragma warning disable IDE1006 // Naming Styles

[Serializable]
public class SerializableStringIntDict : SerializableDictionary<string, int>
{
    internal bool HasExact(string tag, int count)
    {
        if (!ContainsKey(tag))
        {
            return 0 == count;
        }

        return this[tag] == count;
    }
    internal bool HasGreaterThanOrEqual(string tag, int count)
    {
        if (!ContainsKey(tag))
        {
            return 0 >= count;
        }

        return this[tag] >= count;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SerializableStringIntDict))]
public class StringIntDictDrawer : PropertyDrawer
{
    private class StoreFoldStateElement : VisualElement
    {
        public bool fold = false;
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var element = new StoreFoldStateElement();

        var imgui = new IMGUIContainer(() => IMGUI(property, element));
        element.Add(imgui);

        return element;
    }

    private void IMGUI(SerializedProperty property, StoreFoldStateElement element)
    {
        element.fold = EditorGUILayout.Foldout(element.fold, property.displayName);

        if (element.fold)
        {
            var keysProp = property.FindPropertyRelative("keys");
            var valuesProp = property.FindPropertyRelative("values");

            var toRemove = new List<int>();

            if (GUILayout.Button("Add"))
            {
                keysProp.InsertArrayElementAtIndex(keysProp.arraySize);
                var newKey = keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1);
                newKey.stringValue = "";

                valuesProp.InsertArrayElementAtIndex(valuesProp.arraySize);
                var newVal = valuesProp.GetArrayElementAtIndex(valuesProp.arraySize - 1);
                newVal.intValue = 0;
            }

            for (var i = 0; i < keysProp.arraySize; i++)
            {
                var keyProp = keysProp.GetArrayElementAtIndex(i);
                var valueProp = valuesProp.GetArrayElementAtIndex(i);
                if (PairGUI(keyProp, valueProp))
                {
                    toRemove.Add(i);
                }
            }

            for (var i = toRemove.Count - 1; i >= 0; i--)
            {
                keysProp.DeleteArrayElementAtIndex(i);
                valuesProp.DeleteArrayElementAtIndex(i);
            }
        }
    }

    private bool PairGUI(SerializedProperty keyProp, SerializedProperty valueProp)
    {
        var remove = false;

        GUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(keyProp);
        EditorGUILayout.PropertyField(valueProp);

        if (GUILayout.Button("Remove"))
        {
            remove = true;
        }

        GUILayout.EndHorizontal();

        return remove;
    }
}

#endif

#pragma warning restore IDE1006 // Naming Styles
