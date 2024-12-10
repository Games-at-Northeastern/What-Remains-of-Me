using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

#pragma warning disable IDE1006 // Naming Styles

[Serializable]
public class LevelParamDictionary : Dictionary<string, int>, ISerializationCallbackReceiver
{
    [Serializable]
    public class Pair
    {
        [SerializeField] public string String;
        [SerializeField] public int Int;

        public Pair(string String, int Int)
        {
            this.String = String;
            this.Int = Int;
        }

        [CustomPropertyDrawer(typeof(Pair))]
        public class PairDrawer : PropertyDrawer
        {
            public class PairVisualElement : VisualElement
            {
                private readonly SerializedProperty boundPair;

                public PairVisualElement(SerializedProperty boundPair)
                {
                    this.boundPair = boundPair;

                    var stringField = new WaitForBlurField<TextField, string>(this.boundPair.FindPropertyRelative("String"));
                    stringField.style.width = 100f;
                    stringField.style.paddingRight = 4f;
                    var stringLabel = new Label("Param");

                    var stringElement = new VisualElement();
                    stringElement.Add(stringLabel);
                    stringElement.Add(stringField);
                    stringElement.style.flexDirection = FlexDirection.Row;

                    var intField = new WaitForBlurField<IntegerField, int>(this.boundPair.FindPropertyRelative("Int"));
                    intField.style.width = 100f;
                    var intLabel = new Label("Count");

                    var intElement = new VisualElement();
                    intElement.Add(intLabel);
                    intElement.Add(intField);
                    intElement.style.flexDirection = FlexDirection.Row;

                    Add(stringElement);
                    Add(intElement);

                    style.flexDirection = FlexDirection.Row;
                }
            }

            public override VisualElement CreatePropertyGUI(SerializedProperty property) => new PairVisualElement(property);
        }
    }

    [SerializeField] private List<Pair> pairs;
    [SerializeField] private List<string> keys;
    [SerializeField] private bool advanced = true;
    public bool Advanced
    {
        get => advanced;
        set
        {
            advanced = value;

            if (!advanced)
            {
                foreach (var key in Keys)
                {
                    this[key] = 1;
                }
            }
        }
    }

    public LevelParamDictionary()
    {
        keys = new();
        pairs = new();
    }

    public int GetDeferToZero(string param)
    {
        if (TryGetValue(param, out var count))
        {
            return count;
        }

        return 0;
    }

    public void OnBeforeSerialize()
    {
        var i = 0;

        if (advanced)
        {
            foreach (var kvp in this)
            {
                while (pairs[i].String == "")
                {
                    i++;
                }

                if (i < pairs.Count)
                {
                    pairs[i].String = kvp.Key;
                    pairs[i].Int = kvp.Value;
                }
                else
                {
                    pairs.Add(new Pair(kvp.Key, kvp.Value));
                }
                i++;
            }

            for (var j = pairs.Count - 1; j >= i; j--)
            {
                if (pairs[j].String == "")
                {
                    continue;
                }
                pairs.RemoveAt(j);
            }

            keys.Clear();
            foreach (var pair in pairs)
            {
                keys.Add(pair.String);
            }
        }
        else
        {
            foreach (var kvp in this)
            {
                while (keys[i] == "")
                {
                    i++;
                }

                if (i < keys.Count)
                {
                    keys[i] = kvp.Key;
                }
                else
                {
                    keys.Add(kvp.Key);
                }
                i++;
            }

            for (var j = keys.Count - 1; j >= i; j--)
            {
                if (keys[j] == "")
                {
                    continue;
                }
                keys.RemoveAt(j);
            }

            pairs.Clear();
            foreach (var key in keys)
            {
                pairs.Add(new Pair(key, 1));
            }
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        if (advanced)
        {
            foreach (var pair in pairs)
            {
                if (pair.String == "")
                {
                    continue;
                }

                if (!TryAdd(pair.String, pair.Int))
                {
                    pair.String = "";
                }
            }
        }
        else
        {
            var i = -1;
            foreach (var key in keys)
            {
                i++;
                if (key == "")
                {
                    continue;
                }

                if (!TryAdd(key, 1))
                {
                    keys[i] = "";
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(LevelParamDictionary))]
    public class SerializableStringIntDictDrawer : PropertyDrawer
    {
        public class SerializableStringIntDictVisualElement : VisualElement
        {
            private readonly Toggle advancedBind;
            private readonly VisualElement content;
            public SerializableStringIntDictVisualElement(SerializedProperty property)
            {
                style.display = DisplayStyle.Flex;
                style.flexGrow = 1;

                advancedBind = new();
                advancedBind.BindProperty(property.FindPropertyRelative("advanced"));
                advancedBind.style.display = DisplayStyle.None;
                advancedBind.RegisterValueChangedCallback((evt) => Show(property, evt.newValue));
                Add(advancedBind);

                content = new();
                Show(property, property.FindPropertyRelative("advanced").boolValue);

                Add(content);
            }

            private void Show(SerializedProperty property, bool advanced) => ShowList(property, advanced ? "pairs" : "keys");

            private void ShowList(SerializedProperty property, string listPropName)
            {
                content.Clear();

                var keys = new PropertyField()
                {
                    label = property.displayName
                };
                keys.BindProperty(property.FindPropertyRelative(listPropName));

                content.Add(keys);
            }
        }
        public override VisualElement CreatePropertyGUI(SerializedProperty property) => new SerializableStringIntDictVisualElement(property);
    }
}
