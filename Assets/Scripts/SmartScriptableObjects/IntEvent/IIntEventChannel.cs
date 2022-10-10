using System;

namespace SmartScriptableObjects
{
	/// <summary>
	/// Represents the interface for an event channel that has event listeners
	/// that take in an integer as parameter.
	/// </summary>
	public interface IIntEventChannel
	{
		/// <summary>
		/// Calls all the listeners of this event channel.
		/// </summary>
		/// <param name="value">The integer value for the event listeners to take as argument</param>
		void RaiseEvent(int value);

		/// <summary>
		/// Subscribes the given listener to this event channel.
		/// </summary>
		/// <param name="listener">The listener to subscribe</param>
		/// <remarks>
		/// Subscribing a listener that is already subscribed should subscribe two of that listener,
		/// but when event is raised, although there are two of the listeners, there is only the effect
		/// of one of them.
		/// </remarks>
		void SubscribeListener(Action<int> listener);

		/// <summary>
		/// Unsubscribes the given listener to this event channel.
		/// </summary>
		/// <param name="listener">The listener to unsubscribe</param>
		/// <remarks>
		/// Unsubscribing a listener that does not exist does nothing, and unsubscribing a listener
		/// where two of that listeners exist should only remove one of them.
		/// </remarks>
		void UnsubscribeListener(Action<int> listener);
	}
}