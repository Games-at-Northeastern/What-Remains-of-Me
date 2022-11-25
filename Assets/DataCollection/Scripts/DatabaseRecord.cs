using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database Record/String",
    fileName = "New Database Record")]
public class DataBaseRecord : ScriptableObject
{
    public string DataName;
    [TextArea(5,5)]
    public string DataDescription;
    public string Value;

    public virtual string GetValue()
    {
        return Value;
    }
}
