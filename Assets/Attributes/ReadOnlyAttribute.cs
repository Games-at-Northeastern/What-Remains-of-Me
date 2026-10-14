using UnityEditor;
using UnityEngine;
public class ReadOnlyAttribute : PropertyAttribute
{
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect valueRect = EditorGUI.PrefixLabel(position, label);

        // Make it so you can't edit the property value
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(valueRect, property, GUIContent.none);
        EditorGUI.EndDisabledGroup();

        EditorGUI.EndProperty();
    }
}

#endif
