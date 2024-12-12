using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

public class LevelPortalData : ScriptableObject
{
    [SerializeField] private string portalDisplayName;
    [SerializeField] private string foundInScene;

    public string PortalDisplayName => portalDisplayName;
    public string FoundInScene => foundInScene;

    public LevelPortalData() => portalDisplayName = "New Level Portal";
}

#if UNITY_EDITOR

[CustomEditor(typeof(LevelPortalData))]
public class LevelPortalDataEditor : Editor
{
    public class Element : VisualElement
    {
        public Element(SerializedObject obj)
        {
            var textField = new PropertyField();
            textField.BindProperty(obj.FindProperty("portalDisplayName"));
            Add(textField);

            var sceneField = new ObjectField
            {
                objectType = typeof(SceneAsset)
            };
            sceneField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(obj.FindProperty("foundInScene").stringValue);
            sceneField.RegisterValueChangedCallback(evt =>
            {
                obj.FindProperty("foundInScene").stringValue = AssetDatabase.GetAssetPath(evt.newValue);
                obj.ApplyModifiedProperties();
            });
            Add(sceneField);
        }
    }
    public override VisualElement CreateInspectorGUI() => new Element(serializedObject);
}

#endif
