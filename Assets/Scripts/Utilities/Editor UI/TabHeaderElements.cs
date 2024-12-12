using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

#if UNITY_EDITOR
public class TabHeaderElement : VisualElement
{
    private static readonly Color DeactiveColor = new(.1f, .1f, .1f);
    private static readonly Color ActiveColor = new(.4f, .4f, .4f);

    public delegate VisualElement OnSelectTab(int index);
    private readonly List<string> tabNames;
    private readonly List<OnSelectTab> tabFunctions;
    private readonly List<bool> hasAYS;
    private readonly List<bool> areYouSure;

    private readonly VisualElement header;
    private readonly List<UnityEngine.UIElements.Button> buttons;
    private readonly VisualElement content;

    private readonly IntegerField selectedTabBind;
    public Action<int, int> OnTabChange { get; set; }
    public int SelectedTab => selectedTabBind.value;

    private bool largeHeader;
    public bool LargeHeader
    {
        get => largeHeader;
        set
        {
            largeHeader = value;
            RepaintHeader();
        }
    }

    private float HeaderFlexGrow => largeHeader ? 1 : .3f;

    public TabHeaderElement()
    {
        tabNames = new();
        tabFunctions = new();
        hasAYS = new();
        areYouSure = new();

        selectedTabBind = new();
        selectedTabBind.value = -1;

        selectedTabBind.style.display = DisplayStyle.None;
        Add(selectedTabBind);

        style.flexGrow = 1;

        buttons = new();
        header = new VisualElement();
        Add(header);

        content = new VisualElement();
        Add(content);
    }

    public void AddTab(string text, OnSelectTab contentGenerator, bool ays = false)
    {
        tabNames.Add(text);
        tabFunctions.Add(contentGenerator);
        hasAYS.Add(ays);
        areYouSure.Add(false);
    }

    public void RepaintHeader()
    {
        header.Clear();
        header.style.flexDirection = FlexDirection.Row;
        header.style.flexGrow = 1;
        header.style.flexWrap = Wrap.Wrap;

        buttons.Clear();
        for (var i = 0; i < tabNames.Count; i++)
        {
            var button = new UnityEngine.UIElements.Button
            {
                text = tabNames[i],
            };

            button.focusable = false;

            button.style.marginRight = 0;
            button.style.marginLeft = 0;

            if (largeHeader)
            {
                button.style.height = new StyleLength(30);
                button.style.fontSize = 15;
            }

            button.style.flexWrap = Wrap.Wrap;
            button.style.flexGrow = HeaderFlexGrow;

            header.Add(button);
            buttons.Add(button);

            SetButtonStyle(i, false);
        }

        var j = -1;
        foreach (var button in buttons)
        {
            j++;

            if (hasAYS[j])
            {
                button.clicked += () =>
                {
                    var index = buttons.IndexOf(button);

                    if (index == SelectedTab && content.childCount > 0)
                    {
                        return;
                    }

                    areYouSure[index] = !areYouSure[index];
                    if (areYouSure[index])
                    {
                        button.text = "Are you sure?";
                    }
                    else
                    {
                        button.text = tabNames[index];
                        SelectButton(buttons.IndexOf(button));
                    }
                };
            }
            else
            {
                button.clicked += () => SelectButton(buttons.IndexOf(button));
            }

            button.RegisterCallback<MouseLeaveEvent>((evt) =>
            {
                var index = buttons.IndexOf(button);
                areYouSure[index] = false;
                buttons[index].text = tabNames[index];
            });
        }
    }

    private void SelectButton(int index) => selectedTabBind.value = index;

    private void OnSelectButton(int prev, int index)
    {
        if (index == prev && content.childCount > 0)
        {
            return;
        }

        if (prev > -1)
        {
            SetButtonStyle(prev, false);
        }

        SetButtonStyle(index, true);

        content.Clear();
        content.Add(tabFunctions[index].Invoke(index));
    }

    private void SetButtonStyle(int index, bool selected)
    {
        var button = buttons[index];
        button.style.backgroundColor = selected ? ActiveColor : DeactiveColor;
    }

    public void Init(int tab)
    {
        selectedTabBind.SetValueWithoutNotify(tab);
        Init();
    }

    public void Init()
    {
        RepaintHeader();

        selectedTabBind.RegisterValueChangedCallback((evt) =>
        {
            OnSelectButton(evt.previousValue, evt.newValue);
            OnTabChange?.Invoke(evt.previousValue, evt.newValue);
        });

        OnSelectButton(-1, selectedTabBind.value);
    }

    public void BindTabIndexerProperty(SerializedProperty property)
    {
        selectedTabBind.SetValueWithoutNotify(property.intValue);
        selectedTabBind.BindProperty(property);
    }

    internal void SetSelectedTab(int val) => selectedTabBind.value = val;
}

public class TabSelectorElement : VisualElement
{
    private TabHeaderElement header;
    public int SelectedIndex { get; private set; }

    public Action<int> OnSelect { get; set; }

    public TabSelectorElement(SerializedProperty intProp, string label = "", params string[] options) => Init(() => header.BindTabIndexerProperty(intProp), label, options);

    protected TabSelectorElement(int val, string label = "", params string[] options) => Init(() => header.SetSelectedTab(val), label, options);

    private void Init(Action initializeTabIndex, string label = "", params string[] options)
    {
        header = new();

        foreach (var option in options)
        {
            header.AddTab(option, (index) =>
            {
                SelectedIndex = index;
                return new VisualElement();
            });
        }

        header.OnTabChange += (_, index) => OnSelect.Invoke(index);

        initializeTabIndex.Invoke();
        header.Init();

        if (label != "")
        {
            style.flexDirection = FlexDirection.Row;
            Add(new Label(label));
        }

        Add(header);
    }
}

public class BoolTabSelectorElement : TabSelectorElement
{
    public BoolTabSelectorElement(SerializedProperty boolProp, string trueTab, string falseTab, string label = "") : base(boolProp.boolValue ? 0 : 1, label, trueTab, falseTab)
    {
        OnSelect += (ind) =>
        {
            boolProp.boolValue = ind == 0;
            boolProp.serializedObject.ApplyModifiedProperties();
        };
        return;
    }
}

#endif
