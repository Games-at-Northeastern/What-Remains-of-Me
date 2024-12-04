using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public static class LevelTags
{
    private const string TagDataPath = "Assets/Scripts/Levels/Level Tags/tagData.asset";
    public const string TagsFolderPath = "Assets/Scripts/Levels/Level Tags/Tag Storage";
    private static LevelTagData tagData;

    public static LevelTagSO CreateTag()
    {
        var newTag = ScriptableObject.CreateInstance<LevelTagSO>();

        Tags.Add(newTag);

        var path = TagsFolderPath + "/" + Tags.Count + ".asset";

        try
        {
            AssetDatabase.DeleteAsset(path);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
        AssetDatabase.CreateAsset(newTag, path);

        EditorUtility.SetDirty(newTag);
        AssetDatabase.SaveAssetIfDirty(newTag);

        return newTag;
    }

    public static void RemoveTag(LevelTagSO tag)
    {
        Tags.Remove(tag);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(tag));
    }

    public static void MarkDirty()
    {
        EditorUtility.SetDirty(tagData);

        foreach (var tag in Tags)
        {
            EditorUtility.SetDirty(tag);
        }
    }
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
            MarkDirty();
            AssetDatabase.SaveAssetIfDirty(tagData);
            AssetDatabase.Refresh();
        }
    }
}

[CustomEditor(typeof(LevelTagData))]
public class LevelTagDataEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var defaultInspector = new VisualElement();
        InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
        defaultInspector.SetEnabled(false);

        return defaultInspector;
    }
}

[Serializable]
public class LevelTagSO : ScriptableObject
{

}

[CustomPropertyDrawer(typeof(LevelTagSO))]
public class LevelTagSODrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var visuals = new VisualElement();

        var label = new Label(property.displayName);

        visuals.Add(label);

        visuals.style.flexDirection = FlexDirection.Row;

        var objField = new ObjectField();
        objField.SetEnabled(false);
        objField.objectType = typeof(LevelTagSO);
        objField.BindProperty(property);

        visuals.Add(objField);

        var select = new UnityEngine.UIElements.Button
        {
            text = "Select"
        };
        select.clicked += () => SelectMenu(objField);

        visuals.Add(select);

        return visuals;
    }

    private void SelectMenu(ObjectField keyField)
    {
        var selectMenu = new GenericMenu();

        var tags = LevelTags.TagData.Tags;

        List<LevelTagSO> usedTags = new();

        for (var i = 0; i < tags.Count; i++)
        {
            var tag = tags[i];

            if (usedTags.Contains(tag))
            {
                continue;
            }

            selectMenu.AddItem(new GUIContent(tag.name), false,
                () => keyField.value = tag);
        }

        if (selectMenu.GetItemCount() > 0)
        {
            selectMenu.ShowAsContext();
        }
    }
}
