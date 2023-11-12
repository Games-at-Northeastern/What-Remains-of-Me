using UnityEditor;
using UnityEngine;

/// <summary>
/// A small custom property drawer for a range field.
/// </summary>
[CustomPropertyDrawer(typeof(Range))]
public class RangePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var amountRect = new Rect(position.x, position.y, 50, position.height);
        var unitRect = new Rect(position.x + 60, position.y, 50, position.height);

        SerializedProperty min = property.FindPropertyRelative("minValue");
        SerializedProperty max = property.FindPropertyRelative("maxValue");

        EditorGUI.PropertyField(amountRect, min, GUIContent.none);
        EditorGUI.PropertyField(unitRect, max, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
