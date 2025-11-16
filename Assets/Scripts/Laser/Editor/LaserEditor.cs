using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeathLaser))]
public class LaserEditor : Editor
{
    private SerializedProperty laserMode;
    private SerializedProperty mask;


    private bool showParticleOptions = false;
    private SerializedProperty startParticle;
    private SerializedProperty startParticleOffset;

    private SerializedProperty displayLaserEnd;
    private SerializedProperty endParticle;
    private SerializedProperty endParticleOffset;
    private SerializedProperty emissionParticle;


    private bool showDeathOptions = false;

    private SerializedProperty deathTeleporter;
    private SerializedProperty lockoutTime;

    private SerializedProperty laserDistance;
    private SerializedProperty resetSpeed;

    private SerializedProperty onSprite;
    private SerializedProperty offSprite;
    private SerializedProperty onLight;

    private void OnEnable()
    {
        laserMode = serializedObject.FindProperty("laserMode");
        mask = serializedObject.FindProperty("mask");

        startParticle = serializedObject.FindProperty("particles.StartParticle");
        startParticleOffset = serializedObject.FindProperty("particles.StartParticleOffset");

        displayLaserEnd = serializedObject.FindProperty("particles.DisplayLaserEnd");
        endParticle = serializedObject.FindProperty("particles.EndParticle");
        endParticleOffset = serializedObject.FindProperty("particles.EndParticleOffset");
        emissionParticle = serializedObject.FindProperty("particles.EmissionParticle");

        deathTeleporter = serializedObject.FindProperty("death.DeathTeleporter");
        lockoutTime = serializedObject.FindProperty("death.LockoutTime");

        laserDistance = serializedObject.FindProperty("laserDistance");
        resetSpeed = serializedObject.FindProperty("resetSpeed");

        onSprite = serializedObject.FindProperty("onSprite");
        offSprite = serializedObject.FindProperty("offSprite");
        onLight = serializedObject.FindProperty("onLight");
    }


    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(laserMode);
        //If the mode is set to distance
        if (laserMode.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(laserDistance);
        }

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

        EditorGUILayout.PropertyField(mask);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);
        showParticleOptions = EditorGUILayout.BeginFoldoutHeaderGroup(showParticleOptions, "Particles");

        if (showParticleOptions)
        {
            EditorGUILayout.PropertyField(startParticle);
            EditorGUILayout.PropertyField(startParticleOffset);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);
            EditorGUILayout.PropertyField(displayLaserEnd);

            //If the "Display laser end" field is checked
            if (displayLaserEnd.boolValue)
            {
                EditorGUILayout.PropertyField(endParticle);
                EditorGUILayout.PropertyField(endParticleOffset);
                EditorGUILayout.PropertyField(emissionParticle);
            }
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        showDeathOptions = EditorGUILayout.BeginFoldoutHeaderGroup(showDeathOptions, "Death");

        if (showDeathOptions)
        {
            EditorGUILayout.PropertyField(deathTeleporter);
            EditorGUILayout.PropertyField(lockoutTime);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);

        EditorGUILayout.PropertyField(resetSpeed);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2f);
        EditorGUILayout.PropertyField(onSprite);
        EditorGUILayout.PropertyField(offSprite);
        EditorGUILayout.PropertyField(onLight);

        serializedObject.ApplyModifiedProperties();
    }

}
