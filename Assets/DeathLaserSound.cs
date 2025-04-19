using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DeathLaserSound : MonoBehaviour
{

    public static AudioClip laserOnSound;
    public static AudioClip laserOffSound;
    private static AudioSource laserHum;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlayLaserOnSound(Transform t)
    {
        if (laserOnSound != null)
        {
            AudioSource.PlayClipAtPoint(laserOnSound, t.position);
        }
    }

    public static void PlayLaserOffSound(Transform t)
    {
        if (laserOffSound != null)
        {
            AudioSource.PlayClipAtPoint(laserOffSound, t.position);
        }
    }
}
