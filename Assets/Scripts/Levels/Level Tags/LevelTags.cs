using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelTags
{
    private const string TagDataPath = "Assets/Scripts/Levels/Level Tags/tagData.asset";
    private static LevelTagData tagData;

    public static List<LevelTagSO> Tags => TagData.Tags;
    public static SerializableDictionary<Scene, LevelTagDictionary> SceneAcceptTags => TagData.SceneAcceptTags;
    public static LevelTagDictionary GetAcceptTagsByScene(Scene scene)
    {
        if (!scene.IsValid())
        {
            return null;
        }

        if (SceneAcceptTags.ContainsKey(scene))
        {
            return SceneAcceptTags[scene];
        }

        var newDict = new LevelTagDictionary();
        SceneAcceptTags.Add(scene, newDict);
        return newDict;
    }

    public static LevelTagData TagData
    {
        get
        {
            if (tagData == null)
            {
                LoadData();
            }

            return tagData;
        }
    }

    private static void LoadData()
    {
        if (!tagData)
        {
            tagData = AssetDatabase.LoadAssetAtPath<LevelTagData>(TagDataPath);

            if (tagData != null)
            {
                return;
            }

            tagData = ScriptableObject.CreateInstance<LevelTagData>();

            tagData.Tags = new();
            tagData.SceneAcceptTags = new();

            AssetDatabase.CreateAsset(tagData, TagDataPath);
            AssetDatabase.Refresh();
        }
    }
}

public class LevelTagData : ScriptableObject
{
    public List<LevelTagSO> Tags;
    public SerializableDictionary<Scene, LevelTagDictionary> SceneAcceptTags;
}

[Serializable]
public class LevelTagSO : ScriptableObject
{

}
