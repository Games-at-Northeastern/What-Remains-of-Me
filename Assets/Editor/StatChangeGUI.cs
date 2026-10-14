#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using PlayerSettings = PlayerController.PlayerSettings;

[CustomPropertyDrawer(typeof(StatChange))]
public class StatChangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty fieldProp = property.FindPropertyRelative(nameof(StatChange.fieldName));
        SerializedProperty typeProp = property.FindPropertyRelative(nameof(StatChange.upgradeType));
        SerializedProperty numProp = property.FindPropertyRelative(nameof(StatChange.upgradeNum));

        string[] fields = typeof(PlayerSettings)
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.FieldType == typeof(float) || f.FieldType == typeof(int))
            .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
            .Select(f => f.Name)
            .ToArray();

        // Split the rect horizontally (half-half)
        float dropdownWidth = position.width / 2.5f;
        Rect leftRect = new Rect(position.x, position.y, dropdownWidth - 1, position.height);
        Rect rightRect = new Rect(position.x + dropdownWidth + 1, position.y, dropdownWidth - 1, position.height);
        Rect valueRect = new Rect(position.x + dropdownWidth * 2 + 1, position.y, dropdownWidth / 2 - 1, position.height);

        if (fields.Length == 0) {
            return;
        }

        int fieldIndex = Mathf.Max(0, Array.IndexOf(fields, fieldProp.stringValue));

        fieldIndex = EditorGUI.Popup(leftRect, fieldIndex, fields);

        fieldProp.stringValue = fields[fieldIndex];

        typeProp.enumValueIndex = EditorGUI.Popup(rightRect, typeProp.enumValueIndex, typeProp.enumDisplayNames);

        numProp.floatValue = EditorGUI.FloatField(valueRect, numProp.floatValue);

    }
}
#endif
