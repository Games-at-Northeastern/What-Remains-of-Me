#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
        EditorGUI.LabelField(position, property.stringValue, EditorStyles.boldLabel);
}
#endif
