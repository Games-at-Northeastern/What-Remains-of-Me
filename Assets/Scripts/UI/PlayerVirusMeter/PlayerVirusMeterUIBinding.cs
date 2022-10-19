using System;
using SmartScriptableObjects.FloatEvent;
using UnityEngine;

namespace UI
{
    public sealed class PlayerVirusMeterUIBinding : MonoBehaviour
    {
        [SerializeField] private PlayerVirusMeterUI _playerVirusMeterUIMB;
        [SerializeField] private FloatReactivePropertySO _healthReactivePropertySO;
        
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