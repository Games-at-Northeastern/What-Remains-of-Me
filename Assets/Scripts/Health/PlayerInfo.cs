using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerInfo : ScriptableObject
{
    // any information about the player we want tracked can
    // be stored here.
    public float battery;
    public float maxBattery;
    public float virus;
    public float iframesTime;
}
