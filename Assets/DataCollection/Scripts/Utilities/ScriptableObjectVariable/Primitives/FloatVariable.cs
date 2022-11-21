using UnityEngine;

/// <summary>
/// Float Scriptable Object Variable.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Object Variable/Float",
    fileName = "New Float Variable")]
public class FloatVariable : ScriptableObjectVariable<float>
{
    public void IncrementByDeltaTime()
    {
        Value += Time.deltaTime;
    }
}