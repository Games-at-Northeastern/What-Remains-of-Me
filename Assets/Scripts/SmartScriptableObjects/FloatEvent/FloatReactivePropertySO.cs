using System;
using UniRx;

namespace SmartScriptableObjects.FloatEvent
{
	public class FloatReactivePropertySO : DescriptionBaseSO, IFloatReactiveProperty
	{
		private Action<float> _onEventRaised;
		private readonly IReactiveProperty<float> _reactiveProperty = new FloatReactiveProperty();

		void Awake()
		{
			_reactiveProperty.Subscribe(x => _onEventRaised?.Invoke(x));
		}

		public float Value
		{
			get => _reactiveProperty.Value;
			set => _reactiveProperty.Value = value;
		}

		public void SubscribeListener(Action<float> listener)
		{
			_onEventRaised += listener;
		}

		public void UnsubscribeListener(Action<float> listener)
		{
			_onEventRaised -= listener;
		}
	}
}
