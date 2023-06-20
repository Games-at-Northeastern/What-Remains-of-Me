using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This holds any necessary shared information about current player outlet connections
/// </summary>
[CreateAssetMenu(menuName = "SO Player Outlet Connection")]
public class PlayerOutletConnectionsSO : ScriptableObject
{
    public EnergyControl currentOutletConnection;
}
