using UnityEngine;
using UnityEngine.Events;

namespace SmartScriptableObjects.VoidEvent
{
	/// <summary>
	/// A flexible handler for void events in the form of a MonoBehaviour.
	/// Responses can be connected directly from the Unity Inspector.
	/// </summary>
	public class VoidEventListener : MonoBehaviour
	{
		[SerializeField] private VoidEventChannelSO _channelSO;
		[SerializeField] private UnityEvent _onEventRaised;
		
		private IVoidEventChannel _channel;

		void Awake()
		{
			_channel = _channelSO;
		}

		private void OnEnable()
		{
			_channel?.SubscribeListener(Respond);
		}

		private void OnDisable()
		{
			_channel?.UnsubscribeListener(Respond);
		}

		private void Respond()
		{
			_onEventRaised.Invoke();
		}
	}
}