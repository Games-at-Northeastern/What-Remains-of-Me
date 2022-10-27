using System;

namespace SmartScriptableObjects.FloatEvent
{
	/// <summary>
	/// You can ask for the current float value, and attach listeners that listen to
	/// the new float value when it changes.
	/// </summary>
	public interface IFloatReactiveProperty
	{
		/// <summary>
		/// Get or set the value of this reactive property.
		/// </summary>
		float Value { get; set; }
		
		/// <summary>
		/// Subscribes the given listener to this event channel.
		/// </summary>
		/// <param name="listener">The listener to subscribe</param>
		/// <remarks>
		/// Subscribing a listener that is already subscribed should subscribe two of that listener,
		/// but when event is raised, although there are two of the listeners, there is only the effect
		/// of one of them.
		/// </remarks>
		void SubscribeListener(Action<float> listener);

		/// <summary>
		/// Unsubscribes the given listener to this event channel.
		/// </summary>
		/// <param name="listener">The listener to unsubscribe</param>
		/// <remarks>
		/// Unsubscribing a listener that does not exist does nothing, and unsubscribing a listener
		/// where two of that listeners exist should only remove one of them.
		/// </remarks>
		void UnsubscribeListener(Action<float> listener);
	}
}
