using System;

namespace SmartScriptableObjects.VoidEvent
{
	/// <summary>
	/// Represents the interface for an event channel that has event listeners
	/// that do not take in anything as parameter.
	/// </summary>
	public interface IVoidEventChannel
	{
		/// <summary>
		/// Calls all the listeners of this event channel.
		/// </summary>
		public void RaiseEvent();
		
		/// <summary>
		/// Subscribes the given listener to this event channel.
		/// </summary>
		/// <param name="listener">The listener to subscribe</param>
		/// <remarks>
		/// Subscribing a listener that is already subscribed should subscribe two of that listener,
		/// but when event is raised, although there are two of the listeners, there is only the effect
		/// of one of them.
		/// </remarks>
		void SubscribeListener(Action listener);

		/// <summary>
		/// Unsubscribes the given listener to this event channel.
		/// </summary>
		/// <param name="listener">The listener to unsubscribe</param>
		/// <remarks>
		/// Unsubscribing a listener that does not exist does nothing, and unsubscribing a listener
		/// where two of that listeners exist should only remove one of them.
		/// </remarks>
		void UnsubscribeListener(Action listener);
	}
}
