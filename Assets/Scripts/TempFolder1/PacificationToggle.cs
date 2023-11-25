using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is a toggle that fires upon a certain amount of virus being reached, either by going
/// above the given amount or below the given amount.
/// </summary>
public class PacificationToggle : ATriggerToggle
{
    [SerializeField, Tooltip("Whenever ToggleableElements fires, this event fires with the opposite value.")]
    private UnityEvent<bool> InvertedToggleableElements;
    [SerializeField]
    private float _pacifiedWhenBelow = 25f;

    private bool _isToggleActive;
    private bool _wasToggleActive;

    protected override void Awake() => OnVirusChange.AddListener(ProcessChange);

    private void Start() => FireEvent(_enabledOnStart);

    protected override void FireEvent(bool state)
    {
        base.FireEvent(state);
        InvertedToggleableElements.Invoke(!state);
    }

    // The gameobject is "pacified" when the energy enters below a certain value.
    // The event fires when state changes. It fires with a value of true when pacified.
    private void ProcessChange(float energy)
    {
        _isToggleActive = GetVirus() <= _pacifiedWhenBelow;

        // if the state is different than the frame before it...
        if (_isToggleActive != _wasToggleActive)
        {
            // fire the event with the new state and record this new state
            FireEvent(_isToggleActive);
            _wasToggleActive = _isToggleActive;
        }
    }
}
