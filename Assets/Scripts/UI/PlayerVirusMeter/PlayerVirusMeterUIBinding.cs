using SmartScriptableObjects.FloatEvent;
using UnityEngine;

namespace UI.PlayerVirusMeter
{
    /// <summary>
    /// Allows the player virus meter UI to listen to the player's virus value.
    /// </summary>
    /// <remarks>
    /// This implementation does not implement an interface because the behavioural
    /// promises does not occur through methods.
    /// </remarks>
    public sealed class PlayerVirusMeterUIBinding : MonoBehaviour
    {
        #region DependencyInjection
        [SerializeField] private PlayerVirusMeterUI _playerVirusMeterUIMB;
        [SerializeField] private FloatReactivePropertySO _healthReactivePropertySO;
        #endregion
        
        private IPlayerVirusMeterUI _playerVirusMeterUI;
        private IFloatReactiveProperty _healthReactiveProperty;

        void Awake()
        {
            _playerVirusMeterUI = _playerVirusMeterUIMB;
            _healthReactiveProperty = _healthReactivePropertySO;
        }

        private void OnEnable()
        {
            _healthReactiveProperty.SubscribeListener(_playerVirusMeterUI.SetVirusPercentage);
        }

        private void OnDisable()
        {
            _healthReactiveProperty.UnsubscribeListener(_playerVirusMeterUI.SetVirusPercentage);
        }
    }
}