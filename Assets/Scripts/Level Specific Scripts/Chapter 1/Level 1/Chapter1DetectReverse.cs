using UnityEngine;

public class Chapter1DetectReverse : MonoBehaviour
{
    [SerializeField] private bool forceReverse = false;
    [SerializeField] private LevelTagSO voiceBoxTag;
    private void Awake()
    {
        if (forceReverse)
        {
            UpgradeHandler.SetVoiceBox(true);
        }

        if (UpgradeHandler.HasVoiceBox)
        {
            LevelManager.Tags.Add(voiceBoxTag, 1);
        }
    }
}
