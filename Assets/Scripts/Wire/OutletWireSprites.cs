using System;
using UnityEngine;
using UnityEngine.Tilemaps;

//When more wire tile maps are added this will be used to easily swap the kind of wire being used
[CreateAssetMenu(fileName = "Outlet Wire Sprites", menuName = "Outlet Wire Sprites")]
public class OutletWireSprites : ScriptableObject
{
    public TileBase EmptyWire;
    public TileBase CleanEnergyWire;
    public TileBase VirusEnergyWire;
}
