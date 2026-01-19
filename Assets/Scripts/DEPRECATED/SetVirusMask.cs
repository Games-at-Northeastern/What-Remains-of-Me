using UnityEngine;
namespace UI.PlayerBatteryMeter
{

    public class SetVirusMask : MonoBehaviour
    {

        [SerializeField] private RectTransform _maskRectTransform;
        [SerializeField] private float _maxMaskWidth;

        private void Update()
        {

            _maskRectTransform.sizeDelta = new Vector2(EnergyManager.Instance.Virus / 2.62f, _maskRectTransform.sizeDelta.y);
            Debug.Log(EnergyManager.Instance.Virus);
        }
    }


}
