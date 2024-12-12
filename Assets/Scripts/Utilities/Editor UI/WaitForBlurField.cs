using UnityEngine.UIElements;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

#if UNITY_EDITOR
public class WaitForBlurField<TValueElement, TValueType> : VisualElement where TValueElement : BaseField<TValueType>, new()
{
    private readonly TValueElement visuals;
    private readonly TValueElement bind;

    public WaitForBlurField()
    {
        visuals = new();
        bind = new();

        bind.RegisterValueChangedCallback((evt) => visuals.SetValueWithoutNotify(evt.newValue));
        bind.style.display = DisplayStyle.None;

        visuals.RegisterCallback<BlurEvent>((evt) => bind.value = visuals.value);

        Add(visuals);
        Add(bind);
    }
    public WaitForBlurField(SerializedObject serializedObject) : this() => Bind(serializedObject);
    public WaitForBlurField(SerializedProperty property) : this() => BindProperty(property);

    public void Bind(SerializedObject serializedObject) => bind.Bind(serializedObject);

    public void BindProperty(SerializedProperty property) => bind.BindProperty(property);
}

#endif
