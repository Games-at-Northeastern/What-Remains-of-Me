using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmartScriptableObjects
{
	/// <summary>
	/// The scriptable object and unity action implementation for int event channels.
	/// Thus, the events that listen to this event channel all have an int argument.
	/// </summary>
	[CreateAssetMenu(menuName = "SO Events/New Int Event Channel")]
	public class IntEventChannelSO : DescriptionBaseSO, IIntEventChannel
	{
		private event Action<int> _onEventRaised;

		public void RaiseEvent(int value)
		{
			_onEventRaised?.Invoke(value);
		}

		public void SubscribeListener(Action<int> listener)
		{
			_onEventRaised += listener;
		}

		public void UnsubscribeListener(Action<int> listener)
		{
			_onEventRaised -= listener;
		}
	}
}