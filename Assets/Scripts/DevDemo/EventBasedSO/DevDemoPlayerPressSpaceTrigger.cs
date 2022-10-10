using System;
using SmartScriptableObjects;
using SmartScriptableObjects.VoidEvent;
using UnityEngine;
using UnityEngine.Serialization;

namespace DevDemo.EventBasedSO
{
	/// <summary>
	/// This monobehaviour is for dev demo purposes only.
	/// There is a ridiculous quantity of inline comments because this is meant to be
	/// instructive.
	/// </summary>
	public class DevDemoPlayerPressSpaceTrigger : MonoBehaviour
	{
		// This monobehaviour needs a reference to the void event channel, otherwise,
		// it has no notion of events.
		// The reason why it is referring to the interface as opposed to the
		// scriptable object implementation is to follow the "D" in the SOLID principle.
		// The "D" stands for dependency inversion principle, which says that you should
		// depend on the abstraction/interface, not the implementation.
		// So if we do decide to abandon the notion of scriptable objects in our
		// event-based architecture, all we have to do is create a new class
		// that implements the same interface. The only thing that changes in this class
		// is how the _playerPressedSpaceEventChannel field is going to be initialized.
		private IVoidEventChannel _playerPressedSpaceVoidEventChannel;

		// Unfortunately, interfaces do not show up in the inspector, so we can't just
		// drag and drop our channel event scriptable object into the IVoidEventChannel field
		// in the inspector. The work around is to have another field that takes in the implementation
		// of that interface. Since scriptable objects are serializable, they will show up in the
		// inspector. If you are thinking that this is not worth the effort because
		// having two more lines of code is impossibly difficult, and we should just
		// take in the concrete implementation from the get go, be my guest. If we do decide
		// to change implementations for our event-based architecture, the future developers will suffer.
		// Working with interfaces also forces users of the interface to have the right level
		// of coupling, but working with implementations give future users of the implementations
		// to tightly couple their code.
		// There are assets out there that remove the need for this boilerplate code, like Odin Inspector,
		// but they are either not worth the trouble or cost a lot of money.
		// For the purposes of this dev demo, I will be sticking to the dev demo.
		[FormerlySerializedAs("_playerPressedSpaceEventChannelSO")] [SerializeField] private VoidEventChannelSO _playerPressedSpaceVoidEventChannelSO;

		// It is by convention that more privileged/accesible fields go above the less
		// less privileged/accessible fields, and so _playerPressedSpaceEventChannel should
		// go below _playerPressedSpaceEventChannelSO. But I had to switch their order for demonstration
		// purposes.

		[SerializeField] private IntEventChannelSO _playerPressedSpaceIntEventChannelSO;

		private IIntEventChannel _playerPressedSpaceIntEventChannel;
		
		void Awake()
		{
			// The interface version of the field needs to be initialized.
			_playerPressedSpaceVoidEventChannel = _playerPressedSpaceVoidEventChannelSO;
			_playerPressedSpaceIntEventChannel = _playerPressedSpaceIntEventChannelSO;
		}
		
		void Update()
		{
			if (Input.GetKeyDown("space"))
			{
				// The raise event method tells the event channel "hey, please call all of
				// your listeners", and the event channel will do just that.
				_playerPressedSpaceVoidEventChannel.RaiseEvent();
				
				// If you are wondering about the question mark, continue reading the comments below.
				_playerPressedSpaceIntEventChannel?.RaiseEvent(UnityEngine.Random.Range(0, 100));
			}
		}

		// So here, the method which you want to call (DebugLogNum) happens to be
		// in the same class as the caller.
		// Notice how one way to create event listeners is to use the event listener monobehaviour.
		// The coloured particle effects use that way.
		// Here, we are immediately telling a method to get subscribed to a channel event. 
		// This is another way. 
		private void OnEnable()
		{
			// When making an event listener class (so, not this class, but for example,
			// "VoidEventListener", you definitely want to use that question mark operator
			// in the line below. That operator is called the null propagation, and it
			// checks if the object before it is null, if it is not, perform dynamic dispatch
			// on it. Otherwise, don't. 
			// The reason why an event listener class needs to have it is because the user
			// of such a class is not obligated to initialize the channel event, hence,
			// opening opportunities for bugs. As an implementor, you do not want that.
			// Though, for our case here, we are not making an event listener, it just
			// so happens that this monobehaviour has an aspect that listens for events.
			// Thus, the burden of initializing the event channel falls on us. If we do not
			// initialize it, and it errors, that's on us. Though, you can use the null
			// propagation operator if you like, it's up to you in this case.
			_playerPressedSpaceIntEventChannel?.SubscribeListener(DebugLogNum);
		}

		private void OnDisable()
		{
			_playerPressedSpaceIntEventChannel?.UnsubscribeListener(DebugLogNum);
		}

		private void DebugLogNum(int num)
		{
			Debug.Log("Here's your number: " + num);
		}
	}
}