using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;
using UnityEngine;

#pragma warning disable IDE1006 // Naming Styles

// used for data that isnt tied to a specific object/scene, like data referenced in scriptable objects
public class StaticData
{
    public static StaticData Instance { get; private set; }

    public static void Save()
    {
        DataHandler.Save(Instance);
    }

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
        Instance = this;

        levelVersions = new();
        sharedData = new();
    }

    //Not original creator writing this, no idea what "level versions" is even supposed to mean, doesn't look like it's actually used anywhere anymore
    // Possible Ticket: look into level versions/scene group data to see what it's used in

    // Save levelVersion data for SceneGroupData objects
    public SerializableDictionary<SceneGroupData, int> levelVersions;

    //Objects that share data
    //Everything is stored as strings for simplicity, add more string parses for different data types as needed
    public SerializableDictionary<string, object> sharedData;

    public void SetSharedData(string id, object data)
    {
        if (sharedData.ContainsKey(id))
        {
            sharedData[id] = data;
        }
        else
        {
            sharedData.Add(id, data);
        }
    }

    //For ease of use we'll cast the data here before handing it off to other scripts
    public T GetSharedData<T>(string id)
    {
        if (!sharedData.ContainsKey(id))
        {
            Debug.LogError("Trying to get shared data that doesn't exist!");
            return default;
        }

        try
        {
            var data = sharedData[id];
            if (data is T referenceData)
            {
                return referenceData;
            }
            else if (data != null && typeof(T).IsValueType)
            {
                return (T)Convert.ChangeType(data, typeof(T));
            }
            else
            {
                Debug.LogError("Wrong data type in shared data! Make sure you're casting the correct data type.");
                return default;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failure to get shared data: " + e);
            return default;
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
