using UnityEngine;

/// <summary>
/// This class is a toggle that fires upon a certain amount of energy being reached, either by going
/// above the given amount or below the given amount.
/// </summary>
public class EnergyToggle : AListenerToggle
{
    [SerializeField, Range(0f, 1f)]
    private float _percentActivation = 1f;

    protected override bool IsToggleActive() => _observed.GetPercentFull() >= _percentActivation;

}
