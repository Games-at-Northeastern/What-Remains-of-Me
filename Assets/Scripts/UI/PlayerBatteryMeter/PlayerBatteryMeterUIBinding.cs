using UniRx;
using UnityEngine;
namespace UI.PlayerBatteryMeter
{

    /// <summary>
    ///     Allows the player battery meter UI to listen to the player's virus value.
    /// </summary>
    /// <remarks>
    ///     This implementation does not implement an interface because the behavioural
    ///     promises does not occur through methods.
    /// </remarks>
    public sealed class PlayerBatteryMeterUIBinding : MonoBehaviour
    {
        #region DependencyInjection

        [SerializeField] private PlayerBatteryMeterUI _playerBatteryMeterUIMB;

        #endregion
        private IReadOnlyReactiveProperty<float> _batteryReactiveProperty;

        private IPlayerBatteryMeterUI _playerBatteryMeterUI;

        private void Awake()
        {
            _playerBatteryMeterUI = _playerBatteryMeterUIMB;
            _batteryReactiveProperty = new ReactiveProperty<float>(EnergyManager.Instance.BatteryPercentage);

            // Get the virus UI to sync up with player virus stat when the scene is loaded
            // This only has to happen once, it will automatically update hereafter
            _playerBatteryMeterUI.SetCurrBatteryPercentage(_batteryReactiveProperty.Value);
        }

        private void OnEnable() => _batteryReactiveProperty.TakeUntilDisable(this).Subscribe(_playerBatteryMeterUI.SetCurrBatteryPercentage);
        //_virusReactiveProperty.TakeUntilDisable(this).Subscribe(_playerBatteryMeterUI.SetCurrVirusPercentage);
    }
}
