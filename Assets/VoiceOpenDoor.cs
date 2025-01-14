using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOpenDoor : Interaction
{
    public AControllable door;

    public override void Execute()
    {
        door.VirusChange(100);
        door.EnergyChange(100);
    }
}
