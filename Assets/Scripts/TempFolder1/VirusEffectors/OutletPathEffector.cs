using UnityEngine;

/// <summary>
/// When the virus in this controller gets large enough, moves the given elements onto the charged path (works for outlets).
/// </summary>
[RequireComponent(typeof(MovingElementControllerScript))]
public class OutletPathEffector : MonoBehaviour
{
    [SerializeField, Tooltip("Auto-assigned to the MovingElementControllerScript component on this gameobject if null")]
    private MovingElementControllerScript _elementController;

    [SerializeField] private MovingElementScript[] _movingElements;

    [SerializeField] private MovingElementPathScript _chargedPath;

    [SerializeField, Range(0f, 1f), Tooltip("When the percentage of virus hits above this number, this effect activates.")]
    private float _doVirusEffectAt = 0f;

    private bool _hasChangedPath;

    private void Start()
    {
        if (_elementController == null)
        {
            _elementController = GetComponent<MovingElementControllerScript>();
        }

        _elementController.OnVirusChange.AddListener(UpdateVirusPercentageAndApply);

        float? startPercent = _elementController.GetVirusPercent();
        if (startPercent.HasValue)
        {
            UpdateVirusPercentageAndApply(startPercent.Value);
        }
    }

    private void UpdateVirusPercentageAndApply(float virusPercentage)
    {
        if (_hasChangedPath || virusPercentage < _doVirusEffectAt)
        {
            return;
        }

        _hasChangedPath = true;

        foreach (MovingElementScript element in _movingElements)
        {
            element.SetPath(_chargedPath, true);
        }
    }
}
