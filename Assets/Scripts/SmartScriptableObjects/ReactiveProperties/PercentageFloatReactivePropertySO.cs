using System;
using UniRx;
using UnityEngine;

namespace SmartScriptableObjects.FloatEvent
{
	/// <summary>
	/// A scriptable object implementation of the float reactive property interface. Though,
	/// the inspector only allows values between 0 and 1. The float value exists on an
	/// asset level and be injected into any MonoBehaviour or
	/// scriptable object via drag and drop through the inspector.
	/// Note that modifying the value of this float will be clamped between 0 and 1f.
	/// </summary>
	[CreateAssetMenu(menuName = "SO Reactive Properties/Float")]
	public class PercentageFloatReactivePropertySO : DescriptionBaseSO, IReactiveProperty<float>,
		IDisposable, IOptimizedObservable<float>
	{
    [RangeReactiveProperty(0, 1f)]
		[SerializeField] private FloatReactiveProperty _reactiveProperty;

		public IDisposable Subscribe(IObserver<float> observer)
		{
			return _reactiveProperty.Subscribe(observer);
		}

		public float Value
		{
			get => _reactiveProperty.Value;
			set => _reactiveProperty.Value = Mathf.Clamp(value, 0, 1f);
		}

		public bool HasValue => _reactiveProperty.HasValue;

		public void Dispose()
		{
			_reactiveProperty.Dispose();
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return _reactiveProperty.IsRequiredSubscribeOnCurrentThread();
		}
	}
}
