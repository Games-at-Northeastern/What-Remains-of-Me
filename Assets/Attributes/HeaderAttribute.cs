using UnityEditor;
using UnityEngine;
public class SwitchableHeaderAttribute : PropertyAttribute
{
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SwitchableHeaderAttribute))]
public class SwitchableHeaderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
        EditorGUI.LabelField(position, property.stringValue, EditorStyles.boldLabel);
}

#endif
