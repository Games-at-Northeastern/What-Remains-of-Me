using UnityEngine;

public abstract class LevelPortalData : ScriptableObject
{
    [SerializeField] private string portalDisplayName;
    [SerializeField] private SceneGroupData sceneGroup;

    public string PortalDisplayName => portalDisplayName;
    public SceneGroupData SceneGroup => sceneGroup;
    public string GetNextScene() => SceneGroup.GetScene();

    public LevelPortalData() => portalDisplayName = "New Level Portal";
}
