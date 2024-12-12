using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

[CreateAssetMenu(fileName = "ElevatorPortalData", menuName = "SceneTransitions/ElevatorPortalData", order = 1)]
public class ElevatorPortalData : LevelPortalData
{
    [SerializeField] private List<LevelPortalData> layout;

    public List<LevelPortalData> Layout
    {
        get => layout;
        set => layout = value;
    }

    public ElevatorPortalData()
    {
        layout = new();
        layout.Add(this);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ElevatorPortalData))]
public class ElevatorPortalDataDrawer : Editor
{
    private class ListItemElement : VisualElement
    {
        private ObjectField objField;
        private Label thisLabel;
        public void Init(SerializedProperty prop, LevelPortalData lvp)
        {
            Clear();

            thisLabel = new Label(" - This Elevator - ");
            Add(thisLabel);
            objField = new ObjectField
            {
                label = "Elevator Level",
                objectType = typeof(LevelPortalData)
            };
            objField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == lvp)
                {
                    SetIsThis();
                }
                else
                {
                    Reset();
                }
            });
            objField.BindProperty(prop);
            Add(objField);
        }

        public void SetIsThis()
        {
            thisLabel.style.display = DisplayStyle.Flex;
            objField.style.display = DisplayStyle.None;
            style.flexGrow = 1;
            style.backgroundColor = new Color(.1f, .1f, .1f);
            style.alignItems = Align.Center;
            style.justifyContent = Justify.Center;
        }

        public void Reset(bool unbind = false)
        {
            thisLabel.style.display = DisplayStyle.None;
            objField.style.display = DisplayStyle.Flex;
            style.flexGrow = 1;
            style.backgroundColor = new Color(.4f, .4f, .4f);
            style.alignItems = Align.Stretch;
            style.justifyContent = Justify.SpaceBetween;

            if (unbind)
            {
                objField.Unbind();
            }
        }
    }
    public override VisualElement CreateInspectorGUI()
    {
        var main = new VisualElement();

        var baseElement = new LevelPortalDataEditor.Element(serializedObject);
        main.Add(baseElement);

        var thisObj = serializedObject.targetObject as ElevatorPortalData;

        // list ui

        var itemsSourceProp = serializedObject.FindProperty("layout");
        static VisualElement makeItem() => new ListItemElement();
        void bindItem(VisualElement e, int i) => (e as ListItemElement).Init(itemsSourceProp.GetArrayElementAtIndex(i), thisObj);

        var layoutView = new ListView(thisObj.Layout, 25, makeItem, bindItem)
        {
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            //virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            showAddRemoveFooter = true,
            showAlternatingRowBackgrounds = AlternatingRowBackground.All,
            showBorder = true
        };

        layoutView.unbindItem = (e, i) => (e as ListItemElement).Reset(true);

        layoutView.itemsAdded += (i) =>
        {
            foreach (var index in i)
            {
                thisObj.Layout[index] = null;
            }
            serializedObject.UpdateIfRequiredOrScript();
            layoutView.ScrollToItem(-1);
        };

        layoutView.itemsRemoved += (_) =>
        {
            foreach (var lpd in thisObj.Layout)
            {
                if (lpd == thisObj)
                {
                    return;
                }
            }
            thisObj.Layout.Add(thisObj);
        };

        layoutView.horizontalScrollingEnabled = false;
        layoutView.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;

        main.Add(layoutView);

        return main;
    }
}

#endif
