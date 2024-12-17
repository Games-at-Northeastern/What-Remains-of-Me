using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[CreateAssetMenu(fileName = "SceneGroupData", menuName = "SceneTransitions/SceneGroupData", order = 1)]
public class SceneGroupData : ScriptableObject
{
    [SerializeField] private List<string> scenes;

    public string GetScene()
    {
        if (scenes.Count <= LevelVersion)
        {
            throw new System.Exception("levelVersion exceeds scene count in scene group data");
        }

        return scenes[LevelVersion];
    }

    public int LevelVersion
    {
        get
        {
            var data = StaticData.Get();
            if (!data.levelVersions.ContainsKey(this))
            {
                data.levelVersions.Add(this, 0);
            }
            return data.levelVersions[this];
        }
        set
        {
            var data = StaticData.Get();
            if (!data.levelVersions.ContainsKey(this))
            {
                data.levelVersions.Add(this, value);
                return;
            }
            data.levelVersions[this] = value;
        }
    }
    public void IterateVersion()
    {
        if (LevelVersion + 1 < scenes.Count)
        {
            LevelVersion++;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor((typeof(SceneGroupData)))]
public class SceneGroupEditor : Editor
{
    private class SceneElement : VisualElement
    {
        public SceneElement(SerializedObject obj, SerializedProperty prop)
        {
            var sceneField = new ObjectField
            {
                objectType = typeof(SceneAsset),
                value = AssetDatabase.LoadAssetAtPath<SceneAsset>(prop.stringValue)
            };
            sceneField.RegisterValueChangedCallback(evt =>
            {
                prop.stringValue = AssetDatabase.GetAssetPath(evt.newValue);
                obj.ApplyModifiedProperties();
            });

            Add(sceneField);
        }
    }

    private class SimpleList : VisualElement
    {
        private readonly VisualElement header;
        private readonly VisualElement main;
        private readonly SerializedObject obj;
        private readonly SerializedProperty prop;
        public SimpleList(SerializedObject obj, SerializedProperty prop)
        {
            header = new();
            main = new();
            this.obj = obj;
            this.prop = prop;

            var addButton = new UnityEngine.UIElements.Button
            {
                text = "Add"
            };
            addButton.clicked += () =>
            {
                prop.InsertArrayElementAtIndex(prop.arraySize);
                obj.ApplyModifiedProperties();
                Regen();
            };

            var removeButton = new UnityEngine.UIElements.Button
            {
                text = "Remove"
            };
            removeButton.clicked += () =>
            {
                if (prop.arraySize > 0)
                {
                    prop.DeleteArrayElementAtIndex(prop.arraySize - 1);
                    obj.ApplyModifiedProperties();
                    Regen();
                }
            };

            header.Add(addButton);
            header.Add(removeButton);
            Add(header);

            Regen();
            Add(main);
        }

        private void Regen()
        {
            main.Clear();

            foreach (var subProp in prop)
            {
                var sceneField = new SceneElement(obj, subProp as SerializedProperty);
                main.Add(sceneField);
            }
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        var main = new VisualElement();

        main.Add(new SimpleList(serializedObject, serializedObject.FindProperty("scenes")));

        return main;
    }
}
#endif
