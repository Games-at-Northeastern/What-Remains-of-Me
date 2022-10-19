using System;
using UniRx;
using UnityEngine;

namespace SmartScriptableObjects.FloatEvent
{
	[CreateAssetMenu(menuName = "SO Reactive Properties/Float")]
	public class FloatReactivePropertySO : DescriptionBaseSO, IFloatReactiveProperty
	{
		[SerializeField] private FloatReactiveProperty _reactiveProperty;
		
		private Action<float> _onEventRaised;

		void OnEnable()
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
