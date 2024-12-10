using UnityEngine;

public class Chapter1DetectReverse : MonoBehaviour
{
    [SerializeField] private bool forceReverse = false;
    [SerializeField] private string voiceBoxTag;
    private void Awake()
    {
        if (forceReverse)
        {
            UpgradeHandler.SetVoiceBox(true);
        }

        if (UpgradeHandler.HasVoiceBox)
        {
            LevelManager.Parameters.Add(voiceBoxTag, 1);
        }
    }
}
