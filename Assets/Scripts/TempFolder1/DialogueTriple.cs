using UnityEngine;

[System.Serializable]
public struct DialogueTriple
{
    public TextAsset dialogue;

    public Vector2Int playerVirusRange; // not great, but MinMaxSliders are really annoying to figure out. Make it better!
    public Vector2Int targetVirusRange;

    public bool IsWithinRange(float player, float target)
        => UtilityFunctions.ValueInRange(player, playerVirusRange[0], playerVirusRange[1])
        && UtilityFunctions.ValueInRange(target, targetVirusRange[0], targetVirusRange[1]);
}
