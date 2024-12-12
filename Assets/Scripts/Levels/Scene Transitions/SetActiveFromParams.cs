using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using System;
public class SetActiveFromParams : MonoBehaviour
{
    [Serializable]
    public class ParamActivasion
    {
        [SerializeField] private int displaySimplicity = 0;

        [SerializeField] private bool active = true;
        [SerializeField] private bool checkFail = false;

        [SerializeField] private LevelParamDictionary tagCase;
        [SerializeField] private List<GameObject> objects;

        // advanced
        [SerializeField] private bool invertOperation = false;
        [SerializeField] private int matchOperation = 1;
        [SerializeField] private int allNoneAnyCount = 0;
        [SerializeField] private int countMatchOp = 2;
        [SerializeField] private int countMatchNum = 1;

        public bool CheckFail => checkFail;
        public bool Active => active;
        public bool InvertOperation => invertOperation;
        public int AllNoneAnyCount => allNoneAnyCount;
        public int CountMatchOp => CountMatchOp;

        public LevelParamDictionary ParameterCase { get => tagCase; set => tagCase = value; }
        public List<GameObject> Objects { get => objects; set => objects = value; }

        public ParamActivasion() =>
            objects = new List<GameObject>();

        public bool Matches(int req, int count) => MatchOperations[matchOperation].Match(req, count);
        public bool SuccessCountMatches(int count) => MatchOperations[countMatchOp].Match(countMatchNum, count);

        public class MatchOperation
        {
            public delegate bool Operation(int req, int count);
            private readonly Operation operation;

            public MatchOperation(string desc, Operation op, string matchCountDesc)
            {
                OperationDescription = desc;
                operation = op;
                MatchCountDescription = matchCountDesc;
            }

            public bool Match(int req, int count) => operation.Invoke(req, count);

            /*
             * Format:
             * (C): param condition (i.e. "any cases" "all cases" "no cases", etc.)
             * (N): where to place operation not, if needed
             * (G): where to place global not (" its not true that "), if needed
             * 
             * e.g.: for deactivating objects if any parameter count is not >= to req, the whole string would be:
             * "If any tag count is not greater than or equal to the required value, deactivates the specified objects"
             * This classes operationDescription for >= would be:
             * "If (G) (C) is (N) greater than or equal to the required value" (with spaced around special formatters removed)
             */
            public string OperationDescription { get; }

            public string MatchCountDescription { get; }
        }

        public static readonly Dictionary<int, MatchOperation> MatchOperations = new() // WARNING: Never change the keys of existing match operations unless intended, this may invalidate existing advanced conditions
        {
            { 0, new MatchOperation("if(G)(C) the paramater count is(N)greater than the required value", (req, count) => count > req, ", for more than (N) cases,") },
            { 1, new MatchOperation("if(G)(C) the parameter count is(N)greater than or equal to the required value", (req, count) => count >= req, ", for at least (N) cases,") },
            { 2, new MatchOperation("if(G)(C) the parameter count is(N)equal to the required value", (req, count) => count == req, ", for exactly (N) cases,") },
            { 3, new MatchOperation("if(G)(C) the parameter count is(N)less than or equal to the required value", (req, count) => count <= req, ", for at most (N) cases,") },
            { 4, new MatchOperation("if(G)(C) the parameter count is(N)less than the required value", (req, count) => count < req, ", for less than (N) cases,") }
        };
    }

    [SerializeField] private List<ParamActivasion> requirements = new();
    public List<ParamActivasion> Requirements => requirements;

    public void Start()
    {
        foreach (var requirement in requirements)
        {
            if (ParseConditions(requirement, LevelManager.Parameters))
            {
                foreach (var gameObject in requirement.Objects)
                {
                    gameObject.SetActive(requirement.Active);
                }
            }
        }
    }

    public bool ParseConditions(ParamActivasion tagActivasion, LevelParamDictionary activeParams)
    {
        var matchCount = 0;
        var success = tagActivasion.AllNoneAnyCount != 2;
        foreach (var tagReq in tagActivasion.ParameterCase)
        {
            var caseSucceed = ParseCondition(tagActivasion, tagReq, activeParams.GetDeferToZero(tagReq.Key));
            if (caseSucceed)
            {
                matchCount++;
            }

            if (tagActivasion.AllNoneAnyCount == 0) // all
            {
                if (!caseSucceed)
                {
                    success = false;
                    break;
                }
                continue;
            }

            if (tagActivasion.AllNoneAnyCount == 1) // none
            {
                if (caseSucceed)
                {
                    success = false;
                    break;
                }
                continue;
            }

            if (tagActivasion.AllNoneAnyCount == 2) // any
            {
                if (caseSucceed)
                {
                    success = true;
                    break;
                }
                continue;
            }
        }

        if (tagActivasion.AllNoneAnyCount == 3) // count
        {
            success = tagActivasion.SuccessCountMatches(matchCount);
        }

        success = success == !tagActivasion.CheckFail; // global not
        return success;
    }
    public bool ParseCondition(ParamActivasion reqs, KeyValuePair<string, int> req, int paramCount)
    {
        var succeed = reqs.Matches(req.Value, paramCount);
        succeed = succeed == !reqs.InvertOperation;

        return succeed;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SetActiveFromParams.ParamActivasion))]
public class ParamActivasionDrawer : PropertyDrawer
{
    public class Element : VisualElement
    {
        public SerializedProperty BaseProperty { get; set; }
        //public Toggle SSIDAdvancedBind { get; set; }

        public VisualElement GenSimple()
        {
            var element = new VisualElement();

            var ssidAdvProp = BaseProperty.FindPropertyRelative("tagCase").FindPropertyRelative("advanced");
            ssidAdvProp.boolValue = false;
            ssidAdvProp.serializedObject.ApplyModifiedProperties();

            var ssidProp = BaseProperty.FindPropertyRelative("tagCase");
            //SSIDAdvancedBind.value = false;

            // settings

            var setActiveToggle = new Toggle
            {
                text = "Activate/Deactivate (t/f)?"
            };
            setActiveToggle.BindProperty(BaseProperty.FindPropertyRelative("active"));
            element.Add(setActiveToggle);

            var checkFailToggle = new Toggle()
            {
                text = "Check for Fail/Success (t/f)"
            };
            checkFailToggle.BindProperty(BaseProperty.FindPropertyRelative("checkFail"));
            element.Add(checkFailToggle);

            // core
            var ssidField = new PropertyField();
            ssidField.BindProperty(ssidProp);
            element.Add(ssidField);

            var objsField = new PropertyField();
            objsField.BindProperty(BaseProperty.FindPropertyRelative("objects"));
            element.Add(objsField);

            // default advanced settings
            BaseProperty.FindPropertyRelative("invertOperation").boolValue = false;
            BaseProperty.FindPropertyRelative("matchOperation").intValue = 1;
            BaseProperty.FindPropertyRelative("allNoneAnyCount").intValue = 0;
            BaseProperty.FindPropertyRelative("countMatchOp").intValue = 1;
            BaseProperty.FindPropertyRelative("countMatchNum").intValue = 1;

            BaseProperty.serializedObject.ApplyModifiedProperties();

            return element;
        }

        public VisualElement GenAdvanced()
        {
            var element = new VisualElement();

            var ssidAdvProp = BaseProperty.FindPropertyRelative("tagCase").FindPropertyRelative("advanced");
            ssidAdvProp.boolValue = true;
            ssidAdvProp.serializedObject.ApplyModifiedProperties();
            //SSIDAdvancedBind.value = true;

            // switch warning

            var switchWarning = new HelpBox("Switching from advanced mode to simple mode may erase data.", HelpBoxMessageType.Warning);
            switchWarning.style.textShadow = new StyleTextShadow();
            element.Add(switchWarning);

            // operation descriptor

            var operationDescriptor = new HelpBox
            {
                text = "",
                messageType = HelpBoxMessageType.Info
            };

            CalculateOperationDescription(operationDescriptor);
            element.Add(operationDescriptor);

            PropertyField addPropField(SerializedProperty property, string label = "")
            {
                var propField = new PropertyField();

                if (label != "")
                {
                    propField.label = label;
                }

                propField.BindProperty(property);

                propField.RegisterCallback<ChangeEvent<int>>((evt) => CalculateOperationDescription(operationDescriptor));
                propField.RegisterCallback<ChangeEvent<bool>>((evt) => CalculateOperationDescription(operationDescriptor));

                element.Add(propField);
                return propField;
            }

            TabSelectorElement addTabSelField(SerializedProperty property, string label = "", params string[] options)
            {
                var propField = new TabSelectorElement(property, label, options);

                propField.OnSelect += (ind) => CalculateOperationDescription(operationDescriptor);
                element.Add(propField);
                return propField;
            }

            void addBoolTabSelField(SerializedProperty property, string trueTab, string falseTab, string label = "")
            {
                var propField = new BoolTabSelectorElement(property, trueTab, falseTab, label);

                propField.OnSelect += (ind) => CalculateOperationDescription(operationDescriptor);
                element.Add(propField);
            }

            // settings
            addBoolTabSelField(BaseProperty.FindPropertyRelative("active"), "Activate", "Deactivate", "Set Objects To");
            addBoolTabSelField(BaseProperty.FindPropertyRelative("checkFail"), "Fails", "Succeeds", "Apply When Case Validation");

            VisualElement countMatchOpVE = new();
            VisualElement countMatchNumVE = new();

            // advanced settings
            addBoolTabSelField(BaseProperty.FindPropertyRelative("invertOperation"), "On", "Off", "Invert Condition");
            addTabSelField(BaseProperty.FindPropertyRelative("matchOperation"), "Match Operation", ">", ">=", "==", "<=", "<");
            var anacField = addTabSelField(BaseProperty.FindPropertyRelative("allNoneAnyCount"), "Cases Condition", "All", "None", "Any", "Count");

            anacField.OnSelect += (int index) =>
            {
                var ds = index == 3 ? DisplayStyle.Flex : DisplayStyle.None;
                countMatchOpVE.style.display = ds;
                countMatchNumVE.style.display = ds;
            };

            countMatchOpVE = addTabSelField(BaseProperty.FindPropertyRelative("countMatchOp"), "Match Count Operation", ">", ">=", "==", "<=", "<");
            countMatchNumVE = addPropField(BaseProperty.FindPropertyRelative("countMatchNum"), "Match Count Number");

            // core
            addPropField(BaseProperty.FindPropertyRelative("tagCase"));
            addPropField(BaseProperty.FindPropertyRelative("objects"));

            return element;
        }

        // perhaps this should be moves into the TagActivasion class
        private void CalculateOperationDescription(HelpBox box)
        {
            var output = SetActiveFromParams.ParamActivasion.MatchOperations[BaseProperty.FindPropertyRelative("matchOperation").intValue].OperationDescription;

            // - Calculate Condition section -
            // global not
            if (BaseProperty.FindPropertyRelative("checkFail").boolValue)
            {
                output = output.Replace("(G)", " it isnt true that");
            }
            else
            {
                output = output.Replace("(G)", "");
            }

            // param condition
            switch (BaseProperty.FindPropertyRelative("allNoneAnyCount").intValue)
            {
                case 0:
                    output = output.Replace("(C)", ", for all cases,");
                    break;
                case 1:
                    output = output.Replace("(C)", ", for no cases,");
                    break;
                case 2:
                    output = output.Replace("(C)", ", for any case,");
                    break;
                case 3:
                    var countString = SetActiveFromParams.ParamActivasion.MatchOperations[BaseProperty.FindPropertyRelative("countMatchOp").intValue].MatchCountDescription;
                    countString = countString.Replace("(N)", BaseProperty.FindPropertyRelative("countMatchNum").intValue.ToString());
                    output = output.Replace("(C)", countString);
                    break;
                default:
                    output += "[ANACError]";
                    break;
            }

            // operator not
            output = output.Replace("(N)", BaseProperty.FindPropertyRelative("invertOperation").boolValue ? " not " : " ");

            // - Calculate prefix -
            var prefix = (BaseProperty.FindPropertyRelative("active").boolValue ? "Activates" : "Deactivate") + " the specified objects ";

            box.text = "Current Function: " + prefix + output + ".";
        }

        public Element() { }
        public Element(SerializedProperty property) => Initialize(property);
        public void Initialize(SerializedProperty property)
        {
            Reset();

            BaseProperty = property;

            /*SSIDAdvancedBind = new Toggle();
            SSIDAdvancedBind.style.display = DisplayStyle.None;
            SSIDAdvancedBind.BindProperty(property.FindPropertyRelative("tagCase").FindPropertyRelative("advanced"));
            Add(SSIDAdvancedBind);*/

            var tabs = new TabHeaderElement();

            tabs.AddTab("Simple", (_) => GenSimple(), true);
            tabs.AddTab("Advanced", (_) => GenAdvanced());

            tabs.BindTabIndexerProperty(property.FindPropertyRelative("displaySimplicity"));

            tabs.Init();

            Add(tabs);
        }

        public void Reset()
        {
            //SSIDAdvancedBind?.Unbind();
            UnbindAll(this);
            Clear();
            BaseProperty = null;
            //SSIDAdvancedBind = null;
        }

        private void UnbindAll(VisualElement el)
        {
            var unbindable = el as BindableElement;
            unbindable?.Unbind();
            foreach (var child in el.Children())
            {
                UnbindAll(child);
            }
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property) => new Element(property);
}

[CustomEditor(typeof(SetActiveFromParams))]
public class SetActiveFromParamsEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var main = new VisualElement();

        var saftObj = serializedObject.targetObject as SetActiveFromParams;

        var itemsSource = saftObj.Requirements;

        VisualElement makeItem() => new ParamActivasionDrawer.Element();
        void bindItem(VisualElement e, int i) => (e as ParamActivasionDrawer.Element).Initialize(serializedObject.FindProperty("requirements").GetArrayElementAtIndex(i));

        var reqsField = new ListView(itemsSource, -1, makeItem, bindItem)
        {
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            showAddRemoveFooter = true,
            showAlternatingRowBackgrounds = AlternatingRowBackground.All,
            showBorder = true
        };
        reqsField.unbindItem = (e, i) => (e as ParamActivasionDrawer.Element).Reset();

        reqsField.itemsAdded += (i) =>
        {
            foreach (var index in i)
            {
                reqsField.itemsSource[index] = new SetActiveFromParams.ParamActivasion();
            }
            serializedObject.UpdateIfRequiredOrScript();
            //reqsField.ScrollToItem(reqsField.itemsSource.Count - 1);
        };

        reqsField.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;

        main.Add(reqsField);

        return main;
    }
}

#endif
