using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TransitionToNextScene : MonoBehaviour
{
    [SerializeField] private bool overloadNextScene = false;
    [SerializeField] private string nextScenePath;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (overloadNextScene)
            {
                SceneManager.LoadScene(nextScenePath);
            }

            // loads the next scene by getting the index of the current scene and adding 1
            // loads the scene that is the scene after the current scene in the build index.
            // this will cause an error when you get to the last scene in the build index,
            // unless the last scene in the build index never calls this script
            Debug.Log("current scene: " + SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TransitionToNextScene))]
public class TransitionToNextSceneEditor : Editor
{
    private SerializedProperty overload;
    private SerializedProperty path;

    private void Awake()
    {
        overload = serializedObject.FindProperty("overloadNextScene");
        path = serializedObject.FindProperty("nextScenePath");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // script header
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((TransitionToNextScene)target), typeof(TransitionToNextScene), false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(overload);

        if (overload.boolValue)
        {
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path.stringValue);

            EditorGUI.BeginChangeCheck();

            var newScene = EditorGUILayout.ObjectField("Next Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                path.stringValue = newPath;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
