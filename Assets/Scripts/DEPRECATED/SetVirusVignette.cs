using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class SetVirusVignette : MonoBehaviour
{
    [SerializeField] private PlayerInfo AtlasPlayerInfo;

    [SerializeField] private Volume vignetteVolume;
    private EnergyManager energyManager;

    private Vignette vg;
    private void Start()
    {
        vignetteVolume.profile.TryGet(out vg);
        energyManager = PlayerRef.PlayerManager.EnergyManager;
    }


    private void Update() => vg.intensity.value = energyManager.Virus / energyManager.Battery;
}
