using System;
using UniRx;
using UnityEngine;

namespace SmartScriptableObjects.FloatEvent
{
	/// <summary>
	/// A scriptable object implementation of the int reactive property interface. Thus,
	/// the int value exists on an asset level and be injected into any MonoBehaviour or
	/// scriptable object via drag and drop through the inspector.
	/// </summary>
	[CreateAssetMenu(menuName = "SO Reactive Properties/Float")]
	public class IntReactivePropertySO : DescriptionBaseSO, IReactiveProperty<int>,
		IDisposable, IOptimizedObservable<int>
	{
		[SerializeField] private IntReactiveProperty _reactiveProperty;

		public IDisposable Subscribe(IObserver<int> observer)
		{
			return _reactiveProperty.Subscribe(observer);
		}

		public int Value
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
