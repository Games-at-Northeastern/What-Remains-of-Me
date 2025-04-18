using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private string mixerParameter;
    [SerializeField] private float minDB, maxDB, startVolume;

    private void Start() {
        volumeSlider.value = startVolume;
    }

    public void SetVolume(float sliderValue) {
        // Convert slider value (0 to 1) to logarithmic scale for audio mixer (-80dB to 20dB)
        float volumeValue = sliderValue <= 0.001f ? minDB : Mathf.Lerp(minDB, maxDB, Mathf.Pow(sliderValue, 2f));
        if (!audioMixer.SetFloat(mixerParameter, volumeValue)) {
            Debug.Log("Volume parameter not found.");
        }
    }
}
