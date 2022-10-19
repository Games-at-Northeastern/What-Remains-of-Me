using System;
using UniRx;
using UnityEngine;

namespace SmartScriptableObjects.FloatEvent
{
	/// <summary>
	/// A scriptable object implementation of the float reactive property interface. Thus,
	/// the float value exists on an asset level and be injected into any MonoBehaviour or
	/// scriptable object via drag and drop through the inspector.
	/// </summary>
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
