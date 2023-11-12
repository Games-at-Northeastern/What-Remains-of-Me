namespace Levels.Objects.Platform
{
    using UnityEngine;

    /// <summary>
    /// When the virus affecting this element gets large enough, overrides the path and loop type.
    /// </summary>
    public class ChaosEffector : AMovingElementVirusEffector
    {
        [SerializeField] private Transform[] _virusPath;

        [SerializeField] private LoopType _overrideLoopType;

        private bool _shouldRevertPath = true;

        private void Awake()
        {
            if (_doVirusEffectAt >= 0.5f)
            {
                Debug.LogWarning("DoVirusEffectAt var has a really high value! It might not trigger.");
            }
        }

        protected override void AffectMovingElement(MovingElement element) => element.SetTrack(_virusPath, _overrideLoopType, _shouldRevertPath);

        /// <summary>
        /// If we reverted previously and should affect this element OR we affected previously and should revert this element,
        /// flip the _shouldRevertPath so that we do the opposite of what we did previously, and return true.
        /// </summary>
        /// <param name="newVirusPercentage"></param>
        /// <returns></returns>
        protected override bool ShouldDoEffect(float newVirusPercentage)
        {
            if ((_shouldRevertPath && newVirusPercentage >= _doVirusEffectAt) || (!_shouldRevertPath && newVirusPercentage < _doVirusEffectAt))
            {
                // this method is called before AffectPlatform, so whatever value of _shouldRevertPath is swapped to gets used.
                _shouldRevertPath = !_shouldRevertPath;
                return true;
            }

            // otherwise, skip all invocations of this method.
            return false;
        }
    }
}
