using System;
public enum StatChangeType
{
    Additive,
    Multiplicative
}

[Serializable]
public class StatChange
{
    public string fieldName;
    public float upgradeNum;
    public StatChangeType upgradeType;
}
