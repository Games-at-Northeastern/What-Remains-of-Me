using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

[System.Serializable]
public class LevelTagDictionary : SerializableDictionary<LevelTagSO, int>
{
    [SerializeField] private int removePair = -1;
    [SerializeField] private bool addPair = false;
    [SerializeField] private int setKey = -1;
    [SerializeField] private int setKeyVal = -1;
#pragma warning disable IDE0052 // Remove unread private members
    [SerializeField] private int size = 0;
#pragma warning restore IDE0052 // Remove unread private members

    [SerializeField] private List<LevelTagSO> keysWithNull = new();
    [SerializeField] private List<int> valuesWithNull = new();

    public override void OnBeforeSerialize()
    {
        base.OnBeforeSerialize();

        var dictHead = 0;
        var nullHead = 0;

        while (nullHead < keysWithNull.Count)
        {
            if (keysWithNull[nullHead] == null)
            {
                nullHead++;
                continue;
            }

            if (dictHead == Count)
            {
                keysWithNull.RemoveAt(nullHead);
                valuesWithNull.RemoveAt(nullHead);
                continue;
            }

            keysWithNull[nullHead] = keys[dictHead];
            valuesWithNull[nullHead] = values[dictHead];

            dictHead++;
            nullHead++;
        }

        while (dictHead < Count)
        {
            keysWithNull.Add(keys[dictHead]);
            valuesWithNull.Add(values[dictHead]);
        }

        addPair = false;
        removePair = -1;
        setKey = -1;
        setKeyVal = -1;

        size = keysWithNull.Count;
        //Debug.Log(size);
    }
    public override void OnAfterDeserialize()
    {
        Debug.Log("after");
        if (removePair > -1)
        {
            keysWithNull.RemoveAt(removePair);
            valuesWithNull.RemoveAt(removePair);
            removePair = -1;
        }

        if (addPair)
        {
            addPair = false;

            if (keysWithNull.Count < LevelTags.Tags.Count)
            {
                var newKeyVal = null as LevelTagSO;

                keysWithNull.Add(newKeyVal);
                valuesWithNull.Add(1);
            }
        }

        if (setKey > -1)
        {
            keysWithNull[setKey] = LevelTags.Tags[setKeyVal];

            setKey = -1;
            setKeyVal = -1;
        }

        /*for (var i = keysWithNull.Count - 1; i >= 0; i--)
        {
            if (valuesWithNull[i] < 1)
            {
                keysWithNull.RemoveAt(i);
                valuesWithNull.RemoveAt(i);
            }
        }*/

        keys.Clear();
        values.Clear();

        for (var i = 0; i < keysWithNull.Count; i++)
        {
            if (keysWithNull[i] == null)
            {
                continue;
            }

            keys.Add(keysWithNull[i]);
            values.Add(valuesWithNull[i]);
        }

        base.OnAfterDeserialize();
    }
}

[CustomPropertyDrawer(typeof(LevelTagDictionary))]
public class LevelTagDictionaryDrawer : PropertyDrawer
{
    private class LTDElement : VisualElement
    {
        public SerializedProperty BaseProperty { get; set; }
        public SerializedProperty KeysProp { get; set; }
        public SerializedProperty ValuesProp { get; set; }
        public VisualElement Foldout { get; set; }
        public VisualElement PairsElement { get; set; }

        public IntegerField RemovePairBind { get; set; }
        public Toggle AddPairBind { get; set; }
        public IntegerField SetKeyBind { get; set; }
        public IntegerField SetKeyValBind { get; set; }

        public void RemovePair(int index)
        {
            RemovePairBind.value = index;
            PairsElement.RemoveAt(index);

            for (var i = 0; i < PairsElement.childCount; i++)
            {
                var wrapped = PairsElement.ElementAt(i);
                var pair = wrapped.Q<PairElement>();
                pair.Index = i;
            }
        }
        public void AddPair()
        {
            Debug.Log("addP");
            Debug.Log(AddPairBind.binding);
            AddPairBind.value = true;
        }
        public void SetPair(int index, int tagI)
        {
            SetKeyBind.value = index;
            SetKeyValBind.value = tagI;
        }

        public IntegerField SizeBind { get; set; }
    }
    public override VisualElement CreatePropertyGUI(SerializedProperty prop)
    {
        var lTDElement = new LTDElement
        {
            BaseProperty = prop
        };

        var property = lTDElement.BaseProperty;

        lTDElement.KeysProp = property.FindPropertyRelative("keysWithNull");
        lTDElement.ValuesProp = property.FindPropertyRelative("valuesWithNull");

        var removePairProp = property.FindPropertyRelative("removePair");
        var addPairProp = property.FindPropertyRelative("addPair");
        var setKeyProp = property.FindPropertyRelative("setKey");
        var setKeyValProp = property.FindPropertyRelative("setKeyVal");

        lTDElement.RemovePairBind = FakeElem<IntegerField>(removePairProp, lTDElement);
        lTDElement.AddPairBind = FakeElem<Toggle>(addPairProp, lTDElement);
        lTDElement.SetKeyBind = FakeElem<IntegerField>(setKeyProp, lTDElement);
        lTDElement.SetKeyValBind = FakeElem<IntegerField>(setKeyValProp, lTDElement);

        lTDElement.SizeBind = FakeElem<IntegerField>(property.FindPropertyRelative("size"), lTDElement);
        lTDElement.SizeBind.RegisterValueChangedCallback((evt) =>
        {
            if (lTDElement.PairsElement.childCount < evt.newValue)
            {
                for (var i = lTDElement.PairsElement.childCount; i < evt.newValue; i++)
                {
                    AddPairUnit(lTDElement, i);
                }
                return;
            }

            for (var i = lTDElement.PairsElement.childCount - 1; i > evt.newValue - 1; i--)
            {
                lTDElement.PairsElement.RemoveAt(i);
            }
            return;
        });

        CalculateGUI(lTDElement);

        return lTDElement;
    }

    private T FakeElem<T>(SerializedProperty prop, VisualElement holder) where T : VisualElement, IBindable, new()
    {
        var valueField = new T();
        valueField.BindProperty(prop);
        //valueField.Add(new Label(prop.displayName));
        valueField.style.visibility = Visibility.Hidden;
        holder.Add(valueField);
        return valueField;
    }

    private void CalculateGUI(LTDElement lTDElement)
    {
        //lTDElement.Clear();

        var property = lTDElement.BaseProperty;

        var foldout = new Foldout()
        {
            text = lTDElement.BaseProperty.displayName,
            viewDataKey = lTDElement.name + "datakey"
        };

        lTDElement.PairsElement = new VisualElement();

        // display add

        var addButton = new UnityEngine.UIElements.Button()
        {
            text = "Add"
        };

        addButton.clicked += () => lTDElement.AddPair();

        foldout.Add(addButton);

        // display list

        for (var i = 0; i < lTDElement.KeysProp.arraySize; i++)
        {
            AddPairUnit(lTDElement, i);
        }

        lTDElement.Foldout = foldout;
        lTDElement.Foldout.Add(lTDElement.PairsElement);
        lTDElement.Add(lTDElement.Foldout);
    }

    private void AddPairUnit(LTDElement lTDElement, int i)
    {
        var property = lTDElement.BaseProperty;

        var key = lTDElement.KeysProp.GetArrayElementAtIndex(i);
        var value = lTDElement.ValuesProp.GetArrayElementAtIndex(i);
        var pairElement = CreateIndividualPairGUI(lTDElement, key, value);
        pairElement.Index = i;

        var wrapped = WrapPair(lTDElement, lTDElement.KeysProp, lTDElement.ValuesProp, pairElement, i);

        lTDElement.PairsElement.Add(wrapped);
    }

    private class PairElement : VisualElement
    {
        public int Index { get; set; }
        public void Remove(LTDElement ltde) => ltde.RemovePair(Index);
    }

    private PairElement CreateIndividualPairGUI(LTDElement ltdElement, SerializedProperty key, SerializedProperty value)
    {
        var pairElement = new PairElement();

        // define pair Element style

        pairElement.style.flexDirection = FlexDirection.Row;
        pairElement.style.justifyContent = Justify.SpaceBetween;

        // add key property

        var keyElement = new ObjectField();
        keyElement.SetEnabled(false);
        keyElement.BindProperty(key);

        // add value property

        var valueElement = new IntegerField();
        valueElement.BindProperty(value);
        valueElement.style.flexGrow = .3f;
        valueElement.RegisterCallback<BlurEvent>((evt) => pairElement.Remove(ltdElement));

        // add to pair property

        pairElement.Add(keyElement);
        pairElement.Add(valueElement);

        return pairElement;
    }

    private VisualElement WrapPair(LTDElement lTDElement, SerializedProperty keys, SerializedProperty values, PairElement pair, int index)
    {
        var wrappedElement = new VisualElement();

        // define style for wrapped element

        wrappedElement.style.flexDirection = FlexDirection.Row;
        wrappedElement.style.justifyContent = Justify.FlexStart;

        // create select button

        var selectButton = new UnityEngine.UIElements.Button
        {
            text = "Select"
        };

        var keyField = pair.ElementAt(0) as ObjectField;
        selectButton.clicked += () => SelectMenu(keyField, lTDElement);

        // create remove button

        var removeButton = new UnityEngine.UIElements.Button()
        {
            text = "Remove"
        };

        removeButton.clicked += () => pair.Remove(lTDElement);

        // add to wrapped

        wrappedElement.Add(pair);
        wrappedElement.Add(selectButton);
        wrappedElement.Add(removeButton);

        return wrappedElement;
    }

    private void SelectMenu(ObjectField keyField, LTDElement lTDElement)
    {
        var selectMenu = new GenericMenu();

        var tags = LevelTags.Tags;

        List<LevelTagSO> usedTags = new();
        var pairs = lTDElement.PairsElement;

        for (var i = 0; i < pairs.childCount; i++)
        {
            var wrapped = pairs.ElementAt(i);
            var pair = wrapped.Q<PairElement>();
            var tagField = pair.Q<ObjectField>();

            if (tagField.value != null)
            {
                usedTags.Add(tagField.value as LevelTagSO);
            }
        }

        for (var i = 0; i < tags.Count; i++)
        {
            var tag = tags[i];

            if (usedTags.Contains(tag))
            {
                continue;
            }

            selectMenu.AddItem(new GUIContent(tag.name), false,
                () => keyField.value = tag);
        }

        if (selectMenu.GetItemCount() > 0)
        {
            selectMenu.ShowAsContext();
        }
    }
}
