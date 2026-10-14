using UnityEngine;
namespace UI.PlayerBatteryMeter
{

    public class SetVirusMask : MonoBehaviour
    {

        [SerializeField] private RectTransform _maskRectTransform;
        [SerializeField] private float _maxMaskWidth;
        private EnergyManager energyManager;

        private void Start() => energyManager = PlayerRef.PlayerManager.EnergyManager;

        private void Update()
        {

            _maskRectTransform.sizeDelta = new Vector2(energyManager.Virus / 2.62f, _maskRectTransform.sizeDelta.y);
            Debug.Log(energyManager.Virus);
        }
    }


}
