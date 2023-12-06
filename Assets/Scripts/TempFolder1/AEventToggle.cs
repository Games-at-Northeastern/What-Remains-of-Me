using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An abstract class for scripts that toggle gameobjects under certain conditions.
/// This class provides the basic implementation for the event itself.
/// Used by by AListenerToggles, toggles that trigger upon observations.
/// Used by by TimedToggle, a toggle that triggers periodically.
///
/// Generally, this class should be extended by toggles that trigger w/o observations.
/// For instance, a hitbox-based toggle or a time-based toggle.
/// If you extend this toggle for a trigger that observes something (like player health, for instance),
/// you should extend the AListenerToggle instead.
/// </summary>
public abstract class AEventToggle : MonoBehaviour
{
    [SerializeField]
    protected bool _enabledOnStart = true;

    [SerializeField]
    private UnityEvent<bool> ToggleableElements;

    protected virtual void Start() => FireEvent(_enabledOnStart);

    protected virtual void FireEvent(bool state) => ToggleableElements.Invoke(state);

}
