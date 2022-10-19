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
        [SerializeField] private FloatReactivePropertySO _virusReactivePropertySO;
        #endregion
        
        private IPlayerVirusMeterUI _playerVirusMeterUI;
        private IFloatReactiveProperty _virusReactiveProperty;

        void Awake()
        {
            _playerVirusMeterUI = _playerVirusMeterUIMB;
            _virusReactiveProperty = _virusReactivePropertySO;
        }

        private void OnEnable()
        {
            _virusReactiveProperty.SubscribeListener(_playerVirusMeterUI.SetVirusPercentage);
        }

        private void OnDisable()
        {
            _virusReactiveProperty.UnsubscribeListener(_playerVirusMeterUI.SetVirusPercentage);
        }
    }
}