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
			set => _reactiveProperty.Value = value;
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
