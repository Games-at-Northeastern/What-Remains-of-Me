
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetData", menuName = "Custom/Sprite Sheet Data")]
public class SpriteSheetData : ScriptableObject
{
    public Texture2D spriteSheet;
    public Vector2Int spriteSize;
    public SpriteRenderer[] sprites;
}
