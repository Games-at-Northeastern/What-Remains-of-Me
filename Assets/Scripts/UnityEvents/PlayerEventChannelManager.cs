namespace UnityEvents
{
    using System;
    using SmartScriptableObjects.ReactiveProperties;
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// Managers event channels related to the player, including the player's transform.
    /// The awake method is used to initialize the private fields that have the interfaces
    /// for the event channels. The start and update calls the update reactive properties methods
    /// to update the event channels.
    /// </summary>
    public class PlayerEventChannelManager : MonoBehaviour
    {
        [Header("Player Position")]
        [SerializeField] private Vector3ReactivePropertySO _playerPositionPropertySO;
        [SerializeField] private Transform _playerTransform;

        private IReactiveProperty<Vector3> _playerPositionProperty;

        private void Awake()
        {
            _playerPositionProperty = _playerPositionPropertySO;
        }

        private void Start() => UpdateReactiveProperties();

        private void Update() => UpdateReactiveProperties();

        private void UpdateReactiveProperties()
        {
            _playerPositionProperty.Value = _playerTransform.position;
        }
    }
}
