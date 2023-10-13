namespace Levels.Objects.Platform
{
    using UnityEngine;

    /// <summary>
    /// When the virus affecting this platform gets large enough, overrides the path and loop type.
    /// </summary>
    public class ChaosEffector : APlatformVirusEffector
    {
        [SerializeField] private Transform[] _virusPath;

        [SerializeField, Range(0f, 1f), Tooltip("Reverts back to original path when virus is below this amount.")]
        private float _revertPathWhenBelow = 0.45f;

        [SerializeField] private LoopType _overrideLoopType;

        private void Awake()
        {
            if (_revertPathWhenBelow >= 0.5f)
            {
                Debug.LogWarning("RevertPathWhenBelow var has a really high value! It might not trigger.");
            }

            if (_doVirusEffectAt >= 0.5f)
            {
                Debug.LogWarning("DoVirusEffectAt var has a really high value! It might not trigger.");
            }

            if (_doVirusEffectAt > _revertPathWhenBelow)
            {
                Debug.LogError("DoVirusAt is greater than RevertPathWhenBelow!");
            }
        }

        protected override void AffectPlatform(Platform platform) => platform.SetTrack(_virusPath, _overrideLoopType, _currentVirusPercentage < _revertPathWhenBelow);

    }
}
