namespace Levels.Objects.Platform
{
    using UnityEngine;
    using UnityEngine.VFX;

    [RequireComponent(typeof(MovingElementController))]
    public abstract class AMovingElementVirusEffector : MonoBehaviour
    {
        [SerializeField, Tooltip("Auto-assigned to the MovingElementController component on this gameobject if null")]
        private MovingElementController _elementController;

        [Space(15), Header("Visuals")]

        [SerializeField] private VisualEffect _virusEffect;
        [SerializeField] private bool _doVisualEffect = true;

        [Space(15), Header("Virus Functionality")]

        [SerializeField, Range(0f, 1f), Tooltip("When the percentage of virus hits above this number, this effect activates.")]
        protected float _doVirusEffectAt = 0f;

        protected float _currentVirusPercentage;

        private void Start()
        {
            if (_elementController == null)
            {
                _elementController = GetComponent<MovingElementController>();
            }

            _elementController.OnVirusChange.AddListener(UpdateVirusPercentageAndApply);

            float? startPercent = _elementController.GetVirusPercent();
            if (startPercent.HasValue) {
                UpdateVirusPercentageAndApply(startPercent.Value);
            }
        }

        /// <summary>
        /// Updates the moving element and triggers the effector if the virus percentage is over the doAt amount.
        /// </summary>
        /// <param name="virusPercentage"></param>
        private void UpdateVirusPercentageAndApply(float virusPercentage)
        {
            _currentVirusPercentage = virusPercentage;
            
            UpdateVirusParticles(_currentVirusPercentage);

            if (ShouldDoEffect(virusPercentage))
            {
                _currentVirusPercentage = virusPercentage;
                _elementController.ApplyToAll(AffectMovingElement);
            }
        }

        /// <summary>
        /// Updates the moving element's particle system depending on the amount of virus.
        /// </summary>
        /// <param name="virusPercentage"></param>
        private void UpdateVirusParticles(float virusPercentage)
        {
            if (_doVisualEffect && _virusEffect)
            {
                if (virusPercentage >= _doVirusEffectAt) {
                    //_virusEffect.SetFloat("Density", virusPercentage);
                } else {
                    //_virusEffect.SetFloat("Density", 0f);
                }
            }
        }

        /// <summary>
        /// Affects the given moving element.
        /// </summary>
        /// <param name="element"></param>
        protected abstract void AffectMovingElement(MovingElement element);

        /// <summary>
        /// Given a new percentage of virus (invoked every time it changes), should this effector be activated?
        /// </summary>
        /// <param name="newVirusPercentage"></param>
        /// <returns></returns>
        protected virtual bool ShouldDoEffect(float newVirusPercentage) => newVirusPercentage >= _doVirusEffectAt;

        /// <summary>
        /// Determines whether or not the proportion of virus held by this AMovingElementVirusEffector is at or above
        /// the threshold for virus behavior set in editor.
        /// </summary>
        /// <param name="newVirusPercentage"></param>
        /// <returns></returns>
        public bool VirusThresholdMet() {
            return _currentVirusPercentage >= _doVirusEffectAt;
        }
    }
}
