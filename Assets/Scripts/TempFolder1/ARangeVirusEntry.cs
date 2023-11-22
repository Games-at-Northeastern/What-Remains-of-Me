using UnityEngine;

/// <summary>
/// A "base" class for items that need to store data about two virus-ranges and an associated asset.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ARangeVirusEntry<T> : IRangeVirusEntry
{
    public T asset;

    public Vector2 playerVirusRange; // not great, but MinMaxSliders are really annoying to figure out. Make it better!
    public Vector2 targetVirusRange;

    public bool IsWithinRange(float player, float target)
        => UtilityFunctions.ValueInRange(player, playerVirusRange[0], playerVirusRange[1])
        && UtilityFunctions.ValueInRange(target, targetVirusRange[0], targetVirusRange[1]);
}
