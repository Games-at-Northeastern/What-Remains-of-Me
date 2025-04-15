using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;


//Updates the wires attached to outlets to match their energy level
public class OutletWire : MonoBehaviour
{
    //The amount of energy required in the outlet to turn the wire green
    [SerializeField]
    private float cleanEnergyToActivate;

    //The amount of virus requires in the outlet to turn the wire purple
    [SerializeField]
    private float virusEnergyToActivate;

    [SerializeField]
    private OutletWireSprites sprites;

    private Outlet outlet;


    private enum WireState
    {
        off,
        clean,
        virus
    }

    private WireState currentState;

    private Tilemap tilemap;

    void Start()
    {
        currentState = WireState.off;
        outlet = GetComponent<Outlet>();
        tilemap = GetComponentInChildren<Tilemap>();
    }

    void Update()
    {
        //Prioratize showing virus if the states are conflicting
        var desiredState = WireState.off;
        if (outlet.GetVirus() >= virusEnergyToActivate)
        {
            desiredState = WireState.virus;
        }
        else if (outlet.GetEnergy() >= cleanEnergyToActivate)
        {
            desiredState = WireState.clean;
        }

        if (desiredState != currentState)
        {
            tilemap.SwapTile(GetWireTile(currentState), GetWireTile(desiredState));
            currentState = desiredState;
        }
    }

    private TileBase GetWireTile(WireState state)
    {
        switch (state)
        {
            case WireState.clean:
                return sprites.CleanEnergyWire;
            case WireState.virus:
                return sprites.VirusEnergyWire;
            case WireState.off:
            default:
                return sprites.EmptyWire;
        }
    }
}
