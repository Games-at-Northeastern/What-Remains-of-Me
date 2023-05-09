using UnityEngine;

/// <summary>
/// Int Scriptable Object Variable.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Object Variable/Int",
    fileName = "New Int Variable")]
public class IntVariable : ScriptableObjectVariable<int>
{
    public void Increment()
    {
        Value++;
    }

    public void StoreStringAsInt(string stringToStore)
    {
        Debug.Log("String to Store: " + stringToStore);
        if (stringToStore != "")
        {
            Value = int.Parse(stringToStore);
        }
    }
}