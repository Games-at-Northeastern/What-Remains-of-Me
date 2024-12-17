using UnityEngine;

#pragma warning disable IDE1006 // Naming Styles

// used for data that isnt tied to a specific object/scene, like data referenced in scriptable objects
public class StaticData
{
    private static StaticData instance;
    public static StaticData Get() => instance;

    public static void Save() => DataHandler.Save(instance);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Load()
    {
        var res = DataHandler.Load(); // should set instance through default constructor, but if there are issues with static data not saving, check this
        if (res == default)
        {
            new StaticData();
        }
    }

    private static string fileName = "staticGameSave.json";
    private static FileDataHandler<StaticData> dataHandler;
    private static FileDataHandler<StaticData> DataHandler
    {
        get
        {
            if (dataHandler == null)
            {
                dataHandler = new(Application.persistentDataPath, fileName);
            }

            return dataHandler;
        }
    }

    public StaticData()
    {
        instance = this;

        levelVersions = new();
    }

    // Save levelVersion data for SceneGroupData objects
    public SerializableDictionary<SceneGroupData, int> levelVersions;
}
#pragma warning restore IDE1006 // Naming Styles
