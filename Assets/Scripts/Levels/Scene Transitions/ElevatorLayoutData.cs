using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElevatorLayoutData", menuName = "SceneTransitions/ElevatorLayoutData", order = 1)]
public class ElevatorLayoutData : ScriptableObject
{
    [SerializeField] private List<ElevatorPortalData> elevatorPortals;
    public List<ElevatorPortalData> Portals => elevatorPortals;
}
