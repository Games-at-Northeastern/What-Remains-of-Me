using SmartScriptableObjects.VoidEvent;
using UnityEngine;

namespace DevDemo
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
		private IVoidEventChannel _playerPressedSpaceEventChannel;

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
		[SerializeField] private VoidEventChannelSO _playerPressedSpaceEventChannelSO;

		void Awake()
		{
			// The interface version of the field needs to be initialized.
			_playerPressedSpaceEventChannel = _playerPressedSpaceEventChannelSO;
		}
		
		void Update()
		{
			if (Input.GetKeyDown("space"))
			{
				// The raise event method tells the event channel "hey, please call all of
				// your listeners", and the event channel will do just that.
				_playerPressedSpaceEventChannel.RaiseEvent();
			}
		}
	}
}