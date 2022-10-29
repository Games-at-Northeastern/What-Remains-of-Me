namespace UI.PlayerBatteryMeter
{
    using System;
    using PlayerVirusMeter;
    using SmartScriptableObjects.FloatEvent;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Allows the player battery meter UI to listen to the player's virus value.
    /// </summary>
    /// <remarks>
    /// This implementation does not implement an interface because the behavioural
    /// promises does not occur through methods.
    /// </remarks>
    public sealed class PlayerBatteryMeterUIBinding : MonoBehaviour
    {
        #region DependencyInjection
        [SerializeField] private PlayerBatteryMeterUI _playerBatteryMeterUIMB;
        [SerializeField] private PercentageFloatReactivePropertySO _batteryReactivePropertySO;
        #endregion

        private IPlayerBatteryMeterUI _playerBatteryMeterUI;
        private IReadOnlyReactiveProperty<float> _batteryReactiveProperty;

        void Awake()
        {
            _playerBatteryMeterUI = _playerBatteryMeterUIMB;
            _batteryReactiveProperty = _batteryReactivePropertySO;

            // Get the virus UI to sync up with player virus stat when the scene is loaded
            // This only has to happen once, it will automatically update hereafter
            _playerBatteryMeterUI.SetCurrBatteryPercentage(_batteryReactiveProperty.Value);
        }

        private void OnEnable()
        {
            _batteryReactiveProperty.TakeUntilDisable(this).Subscribe(_playerBatteryMeterUI.SetCurrBatteryPercentage);
        }
    }
}
