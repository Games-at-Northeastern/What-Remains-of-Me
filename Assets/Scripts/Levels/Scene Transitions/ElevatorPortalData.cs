using UnityEngine;

[CreateAssetMenu(fileName = "ElevatorPortalData", menuName = "SceneTransitions/ElevatorPortalData", order = 1)]
public class ElevatorPortalData : LevelPortalData
{
    [SerializeField] private ElevatorLayoutData layoutData;
    public ElevatorLayoutData LayoutData => layoutData;
}
