using UnityEngine;
using UnityEditor;

public class ObjectParallaxTuner : MonoBehaviour
{
    public bool OverrideParallaxX = false;
    public bool SetParallaxX;
    public float ParallaxX;
    public float ParallaxXMultiplier = 1;

    public bool OverrideParallaxY = false;
    public bool SetParallaxY;
    public float ParallaxY;
    public float ParallaxYMultiplier = 1;

    private void Start()
    {
        foreach (var parallax in FindObjectsOfType<ObjectParallax>())
        {
            parallax.ParallaxTuner = this;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectParallaxTuner))]
public class ObjectParallaxTunerEditor : Editor
{
    private SerializedProperty overrideX;
    private SerializedProperty setX;
    private SerializedProperty parallaxX;
    private SerializedProperty multiplierX;

    private SerializedProperty overrideY;
    private SerializedProperty setY;
    private SerializedProperty parallaxY;
    private SerializedProperty multiplierY;

    private void Awake()
    {
        overrideX = serializedObject.FindProperty("OverrideParallaxX");
        setX = serializedObject.FindProperty("SetParallaxX");
        parallaxX = serializedObject.FindProperty("ParallaxX");
        multiplierX = serializedObject.FindProperty("ParallaxXMultiplier");

        overrideY = serializedObject.FindProperty("OverrideParallaxY");
        setY = serializedObject.FindProperty("SetParallaxY");
        parallaxY = serializedObject.FindProperty("ParallaxY");
        multiplierY = serializedObject.FindProperty("ParallaxYMultiplier");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // parallax x ui

        EditorGUILayout.PropertyField(overrideX);

        if (overrideX.boolValue)
        {
            EditorGUILayout.PropertyField(setX);
            EditorGUILayout.PropertyField(parallaxX);
        }

        EditorGUILayout.PropertyField(multiplierX);
        EditorGUILayout.Space();

        // parallax y ui

        EditorGUILayout.PropertyField(overrideY);

        if (overrideY.boolValue)
        {
            EditorGUILayout.PropertyField(setY);
            EditorGUILayout.PropertyField(parallaxY);
        }

        EditorGUILayout.PropertyField(multiplierY);

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
