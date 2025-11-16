#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
public class ShowIfAttribute : PropertyAttribute
{
    public readonly object desiredVal;
    public readonly string varName;

    public ShowIfAttribute(string varName, object desiredVal)
    {
        this.desiredVal = desiredVal;
        this.varName = varName;
    }
}

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    bool showProp;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        Object obj = property.serializedObject.targetObject;
        Type type = obj.GetType();
        object currentValue = null;

        FieldInfo field = type.GetField(showIf.varName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null) {
            currentValue = field.GetValue(obj);
        } else {
            PropertyInfo prop = type.GetProperty(showIf.varName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop != null) {
                currentValue = prop.GetValue(obj);
            }
        }
        bool show = Equals(currentValue, showIf.desiredVal);
        if (show) {
            EditorGUI.PropertyField(position, property, label, true);
            showProp = true;
        } else {
            showProp = false;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (showProp) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        return 0f;

    }
}
#endif
