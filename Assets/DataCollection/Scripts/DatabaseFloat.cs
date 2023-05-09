using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database Record/Float",
    fileName = "New Database Float Record")]
public class DatabaseFloat : DataBaseRecord
{
    public float FloatValue;

    public override string GetValue()
    {
        return FloatValue.ToString();
    }

    public void IncrementByDeltaTime()
    {
        FloatValue += Time.deltaTime;
    }
}
