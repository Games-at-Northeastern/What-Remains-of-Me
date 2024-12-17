using System.Collections.Generic;
using UnityEngine;

public class SetSceneGroupVersionOnTrigger : MonoBehaviour
{
    [SerializeField] private int newVersion;
    [SerializeField] private List<SceneGroupData> groupsToUpdate;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (var sceneGroupData in groupsToUpdate)
            {
                sceneGroupData.LevelVersion = newVersion;
            }
            hasTriggered = true;
        }
    }
}
