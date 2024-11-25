using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

/// <summary>
/// This class provides the functionalities for the LevelManager singleton,
/// which is used to manage the scene and warp the player between scenes.
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region LevelTags
#pragma warning disable IDE1006
    [Serializable]
    public class TagFlags : ISerializationCallbackReceiver
    {
        private readonly BitArray flags;
        private static readonly int numFlags;
        public BitArray AsBits() => flags;

        static TagFlags()
        {
            if (!Tags.initialized)
            {
                Tags.Initialize();
                numFlags = Tags.currentId;
            }
        }

        public TagFlags() => flags = new BitArray(numFlags);

        public bool Has(int flag)
        {
            if (flag >= flags.Count)
            {
                return false;
            }

            return flags.Get(flag);
        }
        public bool Has(Tags tag) => Has(tag.id);

        public void Set(int flag, bool val)
        {
            if (flag >= flags.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            flags.Set(flag, val);
        }
        public void Set(Tags tag, bool val) => Set(tag.id, val);

        public void Add(int flag) => Set(flag, true);
        public void Add(Tags tag) => Set(tag.id, true);
        public void Remove(int flag) => Set(flag, false);
        public void Remove(Tags tag) => Set(tag.id, false);
        public int Count => flags.Count;

        public bool HasFlags(TagFlags otherFlags)
        {
            if (Count != otherFlags.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Count; i++)
            {
                if (otherFlags.Has(i) && !Has(i))
                {
                    return false;
                }
            }

            return true;
        }

        // Serialization
        [SerializeField] public bool[] asBools;
        [SerializeField] public bool changed = false;
        [SerializeField] private bool startedChanging = false;

        public static TagFlags CurrentChanger { get; private set; }

        public void OnBeforeSerialize()
        {
            if (startedChanging)
            {
                CurrentChanger = this;
                startedChanging = false;
            }

            if (changed && asBools != null)
            {
                OnAfterDeserialize();
                changed = false;

                if (CurrentChanger == this)
                {
                    CurrentChanger = null;
                }
            }

            asBools = new bool[numFlags];
            for (var i = 0; i < Count; i++)
            {
                asBools[i] = Has(i);
            }
        }
        public void OnAfterDeserialize()
        {
            for (var i = 0; i < Count; i++)
            {
                Set(i, asBools[i]);
            }
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(TagFlags))]
        public class TagFlagDrawer : PropertyDrawer
        {
            private GenericMenu GenerateMenu(SerializedProperty bools)
            {
                var menu = new GenericMenu();
                Tags.addTagsToTagMenu(menu, bools);
                return menu;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (GUI.Button(position, "Select Level Tags"))
                {
                    property.FindPropertyRelative("startedChanging").boolValue = true;
                    var bools = property.FindPropertyRelative("asBools");
                    GenericMenu menu = GenerateMenu(bools);
                    menu.ShowAsContext();
                }
            }
        }
#endif
    }

    public class Tags
    {
        public static int currentId = 0;
        public int id { get; private set; }

        public static bool initialized = false;
        public static void Initialize() => Initialize(typeof(Tags));

        private static void Initialize(Type type)
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static).OrderBy(field => field.Name))
            {
                if (field.FieldType != typeof(Tags))
                {
                    continue;
                }

                var tag = field.GetValue(null) as Tags;
                tag.InitializeTag();
            }

            foreach (var subType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public).OrderBy(subType => subType.Name))
            {
                Initialize(subType);
            }
        }

        private Tags() => id = -1;

        private void InitializeTag()
        {
            id = currentId;
            currentId++;
        }

        public static class Chapter1
        {
            public static Tags SPAWN_LEFT = new();
            public static Tags SPAWN_RIGHT = new();

            public static class Level1
            {
                public static Tags VOICE_BOX = new();
            }
        }
#if UNITY_EDITOR
        public static void addTagsToTagMenu(GenericMenu menu, SerializedProperty bools) => AddTagsToTagMenu(typeof(Tags), menu, "", bools);

        private class CallbackData
        {
            public bool flagValue;
            public Tags tag;

            public CallbackData(bool flagValue, Tags tag)
            {
                this.flagValue = flagValue;
                this.tag = tag;
            }
        }

        private static void SelectionCallback(object dataObj)
        {
            var data = dataObj as CallbackData;

            if (TagFlags.CurrentChanger is null)
            {
                return;
            }

            TagFlags.CurrentChanger.asBools[data.tag.id] = data.flagValue;
            TagFlags.CurrentChanger.changed = true;
        }

        public static void AddTagsToTagMenu(Type type, GenericMenu menu, string typeDir, SerializedProperty bools)
        {
            typeDir += type.Name + "/";

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType != typeof(Tags))
                {
                    continue;
                }

                var tag = field.GetValue(null) as Tags;
                var on = bools.GetArrayElementAtIndex(tag.id).boolValue;

                menu.AddItem(new GUIContent(typeDir + field.Name), on, SelectionCallback, new CallbackData(!on, tag));
            }

            foreach (var subType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public))
            {
                AddTagsToTagMenu(subType, menu, typeDir, bools);
            }
        }
#endif
    }
#pragma warning restore IDE1006
    #endregion

    [SerializeField] private TagFlags levelTags;
    public TagFlags LevelTags => levelTags;
    // set this on awake, apply according modifications on start

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
