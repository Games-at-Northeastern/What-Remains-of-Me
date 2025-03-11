
using UnityEngine;
using Levels.Object.Platform;

/// <summary>
/// When the virus affecting this element gets large enough, overrides the path and loop type (works for outlets).
/// </summary>
public class OutletEffector : AMovingElementVirusEffector
{
    [SerializedField] private Transform[] _chargedPath; 

    [SerializedField] private LoopType _overrideLoopType; 

    private bool _shouldRevertPath = true; 

    private void Awake()
    {
        if (_doVirusEffectAt >= 0.5f) {
            Debug.LogWarning("DoVirusEffectAt var has a really high value. It might not trigger."); 
        }
    }

    override protected void AffectMovingOutlet(MovingElement element) => 
        element.SetTrack(_chargedPath, _overrideLoopType, _shouldRevertPath); 
    
    /// <summary>
    /// Checks if the effect should be applied based on 
    /// <summary>
    override protected bool ShouldDoEffect(float newChargePercentage)
    {
        if ((_shouldRevertPath && newChargePercentage >= _doVirusEffectAt) || (!_shouldRevertPath && newChargePercentage < _doVirusEffectAt))
        {
            _shouldRevertPath = !_shouldRevertPath;
            return true;
        }

        return false; 
    }
}
