using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database Record/Int",
    fileName = "New Database Int Record")]
public class DatabaseInt : DataBaseRecord
{
    public int IntValue;

    public override string GetValue()
    {
        return IntValue.ToString();
    }

    public void SetValue(int newValue)
    {
        IntValue = newValue;
    }

    public void Increment()
    {
        IntValue++;
    }
}
