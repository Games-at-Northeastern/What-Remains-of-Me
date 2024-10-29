using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is a toggle that fires upon a certain amount of virus being reached, either by going
/// above the given amount or below the given amount.
/// </summary>
public class PacificationToggle : AListenerToggle
{
    [SerializeField, Tooltip("Whenever ToggleableElements fires, this event fires with the opposite value.")]
    private UnityEvent<bool> InvertedToggleableElements;

    [SerializeField] private bool _percentageBased = false;

    [SerializeField]
    private float _pacifiedWhenBelow = 0.4f;

    protected override void FireEvent(bool state)
    {
        base.FireEvent(state); // ToggleableElements.Invoke(state);
        InvertedToggleableElements.Invoke(!state);
    }

    // The gameobject is "pacified" when the energy enters below a certain value.
    // The event fires when state changes. It fires with a value of true when pacified.
    protected override bool IsToggleActive() => (_percentageBased ? _observed.GetPercentVirus() : _observed.GetVirus()) <= _pacifiedWhenBelow;
}
