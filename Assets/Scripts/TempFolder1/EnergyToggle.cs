using UnityEngine;
using UnityEngine.Events;

public class EnergyToggle : AControllable
{
    public UnityEvent<bool> ToggleableElements;

    [SerializeField, Range(0f, 1f)]
    private float _percentActivation = 1f;

    private bool _priorEnabled;
    private bool _enabled;

    private void Awake() => OnEnergyChange.AddListener(ProcessChange);

    private void ProcessChange(float energy)
    {
        _enabled = GetPercentFull() > _percentActivation;

        if (_enabled != _priorEnabled)
        {
            ToggleableElements.Invoke(_enabled);
            _priorEnabled = _enabled;
        }
    }
}
