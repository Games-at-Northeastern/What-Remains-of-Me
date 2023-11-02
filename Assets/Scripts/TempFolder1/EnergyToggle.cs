using UnityEngine;

/// <summary>
/// This class is a toggle that fires upon a certain amount of energy being reached, either by going
/// above the given amount or below the given amount.
/// </summary>
public class EnergyToggle : ATriggerToggle
{
    [SerializeField, Range(0f, 1f)]
    private float _percentActivation = 1f;

    private bool _enabled;
    private bool _priorEnabled;

    protected override void Awake()
    {
        base.Awake();
        OnEnergyChange.AddListener(ProcessChange);
    }


    private void ProcessChange(float energy)
    {
        // should it be enabled this frame?
        _enabled = GetPercentFull() >= _percentActivation;

        // if the state is different than the frame before it...
        if (_enabled != _priorEnabled)
        {
            // fire the event with the new state and record this new state
            FireEvent(_enabled);
            _priorEnabled = _enabled;
        }
    }
}
