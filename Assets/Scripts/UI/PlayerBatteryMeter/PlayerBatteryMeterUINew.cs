namespace UI.PlayerBatteryMeter
{
    using Animation;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    /// <summary>
    /// Handles the UI view of the player's health by scaling the size of the fill (and the virus
    /// overlay of the fill) proportional to the battery percentage.
    /// The transparency and speed of the virus overlay animation decreases and increases respectively
    /// as the virus value increases.
    /// The minimum virus transparency is the least transparent the virus overlay image can go
    /// when the virus is at its highest. If it is too high, it becomes distracting. Too low, and you
    /// don't feel its effect.
    /// The minimum virus animation speed multiplier is the fastest the animation can go when the
    /// virus is at its highest. If it is too low, it becomes distracting to the eye. Too high, and
    /// you don't feel its effect.
    /// </summary>
    public class PlayerBatteryMeterUINew : MonoBehaviour, IPlayerBatteryMeterUI
    {
        [SerializeField] private RectTransform _maskRectTransform;
        [SerializeField] private float _maxMaskWidth;

        [SerializeField] private Image _virusOverlayImage;
        [Range(0.1f, 1f)] [SerializeField] private float _maxVirusUntransparency;

        [SerializeField] private UIImageLoopingAnimation _virusAnimation;
        [Range(0.1f, 1f)] [SerializeField] private float _minVirusAnimationSpeedMultiplier;


        public void SetCurrBatteryPercentage(float percentage)
        {
            _maskRectTransform.sizeDelta = new Vector2((int) Mathf.CeilToInt(percentage * _maxMaskWidth),
                _maskRectTransform.sizeDelta.y);
        }

        public void SetCurrVirusPercentage(float percentage)
        {
            _virusAnimation.SetSpeedMultiplier(1f - ((1 - _minVirusAnimationSpeedMultiplier) * percentage));
            ChangeVirusOverlayImageTransparency(percentage);
        }

        private void ChangeVirusOverlayImageTransparency(float percentage)
        {
            Color tempColor = _virusOverlayImage.color;
            tempColor.a = _maxVirusUntransparency * percentage;
            _virusOverlayImage.color = tempColor;
        }
    }
}
