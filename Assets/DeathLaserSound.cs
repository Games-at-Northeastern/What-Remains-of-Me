using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DeathLaserSound : MonoBehaviour
{




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        laserHum = GetComponent<AudioSource>();
        laserVolume = laserHum.volume;
    }

    public static AudioClip laserOnSound;
    public static AudioClip laserOffSound;
    private static AudioSource laserHum;

    private static float laserVolume;

    public static void PlayLaserOnSound(Transform t)
    {
        if (laserOnSound != null)
        {
            AudioSource.PlayClipAtPoint(laserOnSound, t.position);
        }
        if (laserHum.volume == 0)
        {
            laserHum.volume = laserVolume;
        }
    }

    public static void PlayLaserOffSound(Transform t)
    {
        if (laserOffSound != null)
        {
            AudioSource.PlayClipAtPoint(laserOffSound, t.position);
        }
        if (laserHum.volume > 0)
        {
            laserHum.volume = 0;
        }
    }
}
