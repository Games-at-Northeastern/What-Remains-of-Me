

using System;
using Levels.Objects.Platform;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Splines;

[CustomEditor(typeof(TestMovingObjectScript))]
[CanEditMultipleObjects]
public class TestMovingPlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TestMovingObjectScript movingObject = (TestMovingObjectScript)target;

        SerializedProperty propertyItr = serializedObject.GetIterator();
        propertyItr.NextVisible(true);

        Dictionary<string, Func<TestMovingObjectScript, bool>> varsAndConditions =
            TestMovingObjectScript.conditionalSerializedVars;

        do
        {
            if (varsAndConditions.ContainsKey(propertyItr.name))
            {
                if (varsAndConditions[propertyItr.name](movingObject))
                {
                    EditorGUILayout.PropertyField(propertyItr, true);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(propertyItr, true);
            }
        } while (propertyItr.NextVisible(false));

        serializedObject.ApplyModifiedProperties();


    }
}



