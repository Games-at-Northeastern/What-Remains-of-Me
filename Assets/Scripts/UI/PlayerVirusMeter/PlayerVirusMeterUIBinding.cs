using System;
using UniRx;
using UnityEngine;
namespace UI.PlayerVirusMeter
{

    /// <summary>
    ///     Allows the player virus meter UI to listen to the player's virus value.
    /// </summary>
    /// <remarks>
    ///     This implementation does not implement an interface because the behavioural
    ///     promises does not occur through methods.
    /// </remarks>
    public sealed class PlayerVirusMeterUIBinding : MonoBehaviour
    {
        #region DependencyInjection

        [SerializeField] private PlayerVirusMeterUI _playerVirusMeterUIMB;

        #endregion

        private IPlayerVirusMeterUI _playerVirusMeterUI;
        private IReadOnlyReactiveProperty<float> _virusReactiveProperty;

        private void Awake()
        {
            _playerVirusMeterUI = _playerVirusMeterUIMB;
            _virusReactiveProperty = new ReactiveProperty<float>(EnergyManager.Instance.Virus);
            ;

            // Get the virus UI to sync up with player virus stat when the scene is loaded
            // This only has to happen once, it will automatically update hereafter
            _playerVirusMeterUI.SetCurrVirusPercentage(_virusReactiveProperty.Value);
        }

        private void OnEnable()
        {
            _virusReactiveProperty.TakeUntilDisable(this).Subscribe(_playerVirusMeterUI.SetCurrVirusPercentage);
            _virusReactiveProperty.TakeUntilDisable(this).Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(x =>
                {
                    _playerVirusMeterUI.SetDelayedVirusPercentage(x);
                    _playerVirusMeterUI.SetBarHolderLightsPercentage(x);
                });
        }
    }
}
