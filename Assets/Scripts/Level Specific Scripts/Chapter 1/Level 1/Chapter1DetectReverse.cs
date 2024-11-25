using UnityEngine;

public class Chapter1DetectReverse : MonoBehaviour
{
    [SerializeField] private bool forceReverse = false;
    private void Awake()
    {
        if (forceReverse)
        {
            UpgradeHandler.SetVoiceBox(true);
        }

        if (UpgradeHandler.HasVoiceBox)
        {
            FindObjectOfType<LevelManager>().LevelTags.Add(LevelManager.Tags.Chapter1.Level1.VOICE_BOX);
        }
    }
}
