using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Variable stored at the asset level. Invokes a delegate when the value is 
/// updated.
/// </summary>
/// <typeparam name="T">Type of variable to be stored.</typeparam>
public abstract class ScriptableObjectVariable<T> : ScriptableObject
{
    /// <summary>
    /// Delegate to signal that this ScriptableObjectVariable has been updated.
    /// </summary>
    public ScriptableObjectVariableUpdated VariableUpdated;

    /// <summary>
    /// The value of this variable as a <T>.
    /// </summary>
    [SerializeField] private T _value;

    /// <summary>
    /// The value of this variable as a <T>. Invokes VariableUpdated when set.
    /// </summary>
    public T Value
    {
        get
        {
            return this._value;
        }
        set
        {
            this._value = value;
            VariableUpdated?.Invoke();
        }
    }
}
