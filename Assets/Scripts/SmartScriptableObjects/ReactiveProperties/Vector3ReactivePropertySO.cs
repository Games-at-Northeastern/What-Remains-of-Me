namespace SmartScriptableObjects.ReactiveProperties
{
    using System;
    using UniRx;
    using UnityEngine;

    /// <summary>
	/// A scriptable object implementation of the float reactive property interface. Thus,
	/// the float value exists on an asset level and be injected into any MonoBehaviour or
	/// scriptable object via drag and drop through the inspector.
	/// </summary>
	[CreateAssetMenu(menuName = "SO Reactive Properties/Vector3")]
	public class Vector3ReactivePropertySO : DescriptionBaseSO, IReactiveProperty<Vector3>,
		IDisposable, IOptimizedObservable<Vector3>
	{
		[SerializeField] private Vector3ReactiveProperty _reactiveProperty;

		public IDisposable Subscribe(IObserver<Vector3> observer)
		{
			return _reactiveProperty.Subscribe(observer);
		}

		public Vector3 Value
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
