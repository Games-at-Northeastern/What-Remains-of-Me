using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmartScriptableObjects.VoidEvent
{
	/// <summary>
	/// The scriptable object and unity action implementation for void event channels.
	/// </summary>
	[CreateAssetMenu(menuName = "SO Events/New Void Event Channel")]
	public class VoidEventChannelSO : DescriptionBaseSO, IVoidEventChannel
	{
		private event Action _onEventRaised;

		public void RaiseEvent()
		{
			_onEventRaised?.Invoke();
		}

		public void SubscribeListener(Action listener)
		{
			_onEventRaised += listener;
		}

		public void UnsubscribeListener(Action listener)
		{
			_onEventRaised -= listener;
		}
	}
}