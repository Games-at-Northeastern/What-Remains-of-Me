using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomTileset", menuName = "ScriptableObjects/Custom Tileset")]
public class CustomTileset : ScriptableObject
{
    [SerializeField] public List<CustomTile> Tiles;

    public void AddTile(CustomTile tile) => Tiles.Add(tile);
}
