using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class LevelTagWindow : EditorWindow
{
    private static LevelTagData data;

    [MenuItem("Window/Tools/Level Tags")]
    public static void ShowEditor()
    {
        EditorWindow wnd = GetWindow<LevelTagWindow>();
        wnd.titleContent = new GUIContent("Level Tags");
    }

    #region Tag Type GUI
    private readonly List<int> toRemove = new();

    public VisualElement TagTypesGUI()
    {
        var tagTypesFoldout = new Foldout()
        {
            text = "All Tags",
            viewDataKey = "allleveltagsfold"
        };

        var tagTypesInternal = new IMGUIContainer(TagTypesIMGUI);

        tagTypesFoldout.Add(tagTypesInternal);

        return tagTypesFoldout;
    }
    private void TagTypesIMGUI()
    {
        data = LevelTags.TagData;
        if (data.Tags is null)
        {
            return;
        }

        var add = GUILayout.Button("Add");

        var count = 0;
        foreach (var item in data.Tags)
        {
            TagGUI(item, count);
            count++;
        }

        for (var i = toRemove.Count - 1; i >= 0; i--)
        {
            var index = toRemove[i];
            data.Tags.RemoveAt(index);
            count--;
        }

        if (add)
        {
            var newTag = CreateInstance<LevelTagSO>();
            newTag.name = "Tag" + count;

            data.Tags.Add(newTag);
        }

        toRemove.Clear();
    }

    private bool onList1 = true;

    [FilePath("Assets/Scripts/Levels/Level Tags/listTexHolder.asset", FilePathAttribute.Location.ProjectFolder)]
    private class ListTexHolder : ScriptableSingleton<ListTexHolder>
    {
        private GUIStyle list1;
        [SerializeField] private Texture2D tex1;

        private GUIStyle list2;
        [SerializeField] private Texture2D tex2;

        public GUIStyle List1
        {
            get
            {
                if (list1 == null)
                {
                    if (tex1 == null)
                    {
                        tex1 = new Texture2D(1, 1);
                        tex1.SetPixel(0, 0, new Color(.1f, .1f, .1f));
                        tex1.Apply();
                    }
                    list1 = new GUIStyle();
                    list1.normal.background = tex1;
                    list1.border = new RectOffset(3, 3, 3, 3);
                }
                return list1;
            }
        }

        public GUIStyle List2
        {
            get
            {
                if (list2 == null)
                {
                    if (tex2 == null)
                    {
                        tex2 = new Texture2D(1, 1);
                        tex2.SetPixel(0, 0, new Color(.15f, .15f, .15f));
                        tex2.Apply();
                    }
                    list2 = new GUIStyle();
                    list2.normal.background = tex2;
                    list2.border = new RectOffset(3, 3, 3, 3);
                }

                return list2;
            }
        }
    }

    private int aysN = -1;

    public void TagGUI(LevelTagSO levelTag, int count)
    {
        onList1 = count % 2 == 1;
        EditorGUILayout.BeginHorizontal(onList1 ? ListTexHolder.instance.List1 : ListTexHolder.instance.List2);

        levelTag.name = EditorGUILayout.TextField(levelTag.name);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(aysN == count ? "Are you sure?" : "Remove"))
        {
            if (aysN == count)
            {
                aysN = -1;
                toRemove.Add(count);
            }
            else
            {
                aysN = count;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Individual Level Select

    //private Dictionary<Scene, LevelTagDictionary> sceneAcceptTags;
    [SerializeField] private LevelTagDictionary levelTags;
    [SerializeField] private bool levelChanged = false;
    [SerializeField] private bool levelNull;
    private SerializedObject windowSO;
    private SerializedProperty levelTagProp;
    private SerializedProperty levelChangeProp;
    private SerializedProperty levelNullProp;
    private void Awake()
    {
        //sceneAcceptTags = LevelTags.SceneAcceptTags;
        LoadLevelTags();
        EditorSceneManager.activeSceneChangedInEditMode += (_, _) => LoadLevelTags();

        windowSO = new SerializedObject(this);
    }

    private void LoadLevelTags()
    {
        /*Scene activeScene;
        if (Application.isPlaying)
        {
            activeScene = SceneManager.GetActiveScene();
        }
        else
        {
            activeScene = EditorSceneManager.GetActiveScene();
        }*/

        levelTags = LevelTags.GetAcceptTagsByScene(SceneManager.GetActiveScene());
        levelChanged = true;
        levelNull = levelTags == null;
    }

    public VisualElement IndividualLevelGUI()
    {
        windowSO ??= new SerializedObject(this);
        levelTagProp ??= windowSO.FindProperty("levelTags");
        levelNullProp ??= windowSO.FindProperty("levelNull");

        var visualElement = new VisualElement();

        var propElement = new PropertyField();
        LoadLevelGUI(propElement);

        visualElement.Add(propElement);

        levelChangeProp = windowSO.FindProperty("levelChanged");

        var levelChangeBind = new Toggle();
        levelChangeBind.BindProperty(levelChangeProp);
        levelChangeBind.style.visibility = Visibility.Hidden;

        levelChangeBind.RegisterValueChangedCallback((evt) =>
        {
            if (evt.newValue)
            {
                LoadLevelGUI(propElement);
                levelChangeBind.value = false;
            }
        });

        visualElement.Add(levelChangeBind);

        return visualElement;
    }

    private void LoadLevelGUI(PropertyField propE)
    {
        if (levelNullProp.boolValue)
        {
            propE.Unbind();
        }
        else
        {
            propE.BindProperty(levelTagProp);
        }

        propE.MarkDirtyRepaint();
    }

    #endregion

    private void CreateGUI()
    {
        rootVisualElement.Add(TagTypesGUI());
        rootVisualElement.Add(IndividualLevelGUI());
    }
}
