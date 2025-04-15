
using Levels.Objects.Platform;
using UnityEngine;

/// <summary>
/// When the virus affecting this element gets large enough, overrides the path and loop type (works for outlets).
/// </summary>
public class OutletEffector : AMovingElementVirusEffector
{
    [SerializeField] private Transform[] _chargedPath;

    [SerializeField] private LoopType _overrideLoopType;

    [SerializeField] private MonoBehaviour outletMovementScript;

    private bool _shouldRevertPath = true;

    private void Awake()
    {
        // Debug.Log($"[Awake] Initial _doVirusEffectAt = {_doVirusEffectAt}");

        if (_doVirusEffectAt >= 0.5f)
        {
            Debug.LogWarning("DoVirusEffectAt var has a really high value. It might not trigger.");
        }
    }

    override protected void AffectMovingElement(MovingElement element)
    {
        element.SetTrack(_chargedPath, _overrideLoopType, _shouldRevertPath);
        if (_shouldRevertPath)
        {
            // Revert to regular moving outlet 
            outletMovementScript.enabled = true;
        }
        else 
        {
            // Apply virus effect. 
            outletMovementScript.enabled = false;
        }
    }

    /// <summary>
    /// Checks if the effect should be applied based on 
    /// <summary>
    override protected bool ShouldDoEffect(float newChargePercentage)
    {
        Debug.Log($"[ShouldDoEffect] Checking condition: newChargePercentage = {newChargePercentage}, _doVirusEffectAt = {_doVirusEffectAt}, _shouldRevertPath = {_shouldRevertPath}");

        if ((_shouldRevertPath && newChargePercentage >= _doVirusEffectAt) || 
            (!_shouldRevertPath && newChargePercentage < _doVirusEffectAt))
        {
            _shouldRevertPath = !_shouldRevertPath;
            Debug.Log($"[ShouldDoEffect] outletMovementScript.enabled set to {_shouldRevertPath}");
            return true;
        }

        return false;
    }
}
