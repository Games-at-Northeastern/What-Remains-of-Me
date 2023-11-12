using System;

[Serializable]
public struct Range
{
    public float minValue;
    public float maxValue;

    public Range(float minValue, float maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public float GetValue() => UnityEngine.Random.Range(minValue, maxValue);

    public void ValidateRange()
    {
        if (minValue >= maxValue)
        {
            throw new ArgumentException("Range min cannot be larger than range max!");
        }
    }
}
