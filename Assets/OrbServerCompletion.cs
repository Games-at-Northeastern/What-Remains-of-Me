using Unity.VisualScripting;
using UnityEngine;

public class OrbServerCompletion : MonoBehaviour
{
    [SerializeField] private InteractOnRayDetect[] unlockables;
    public bool isBossBeaten = false;

    private void Update()
    {
        if(isBossBeaten)
        {
            BeatBoss();
        }
    }

    private void BeatBoss()
    {
        for (int i = 0; i < unlockables.Length; i++)
        {
            unlockables[i].enabled = false;
        }
    }
}
