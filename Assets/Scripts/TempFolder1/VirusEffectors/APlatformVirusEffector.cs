namespace Levels.Objects.Platform
{
    using UnityEngine;
    using UnityEngine.VFX;

    [RequireComponent(typeof(MovingElementController))]
    public abstract class APlatformVirusEffector : MonoBehaviour
    {
        [SerializeField] private MovingElementController _platformController;

        [Space(15), Header("Visuals")]

        [SerializeField] private VisualEffect _virusEffect;
        [SerializeField] private bool _doVisualEffect = true;

        [Space(15), Header("Virus Functionality")]

        [SerializeField, Range(0f, 1f), Tooltip("When the percentage of virus hits above this number, this effect activates.")]
        protected float _doVirusEffectAt = 0f;

        protected float _currentVirusPercentage;

        private void Start()
        {
            if (_platformController == null)
            {
                _platformController = GetComponent<MovingElementController>();
            }

            _platformController.OnVirusChange.AddListener(UpdateVirusPercentageAndApply);
        }

        /// <summary>
        /// Updates the platform and triggers the effector if the virus percentage is over the doAt amount.
        /// </summary>
        /// <param name="virusPercentage"></param>
        private void UpdateVirusPercentageAndApply(float virusPercentage)
        {
            _currentVirusPercentage = virusPercentage;

            UpdateVirusParticles(_currentVirusPercentage);

            if (_currentVirusPercentage > _doVirusEffectAt)
            {
                _platformController.ApplyToAll(AffectPlatform);
            }
        }

        /// <summary>
        /// Updates the platform's particle system depending on the amount of virus.
        /// </summary>
        /// <param name="virusPercentage"></param>
        private void UpdateVirusParticles(float virusPercentage)
        {
            if (_doVisualEffect && _virusEffect)
            {
                _virusEffect.SetFloat("Density", virusPercentage);
            }
        }

        /// <summary>
        /// Affects the given platform.
        /// </summary>
        /// <param name="platform"></param>
        protected abstract void AffectPlatform(MovingElement platform);
    }
}
