namespace UI.Animation
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// If you are looping through some keyframes at a constant rate, use this component.
    /// Why not use an animation and an animator controller? There are performance issues
    /// with you use those with UI images.
    /// You are allowed to modify the speed from the outside through a multiplier.
    /// </summary>
    /// <remarks>Inspired by https://gist.github.com/almirage/e9e4f447190371ee6ce9.</remarks>
    public class UIImageLoopingAnimation : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private float secondsBetweenSprite;

        private int _index = 0;
        private float _speedMultiplier = 1f;

        private void Start()
        {
            StartCoroutine(UpdateEveryXSeconds());
        }

        /// <summary>
        /// Sets the speed multiplier such that the seconds between sprite changes is multiplied
        /// by this multiplier.
        /// </summary>
        /// <param name="newSpeedMultiplier">The new multiplier to influence the speed</param>
        public void SetSpeedMultiplier(float newSpeedMultiplier)
        {
            _speedMultiplier = newSpeedMultiplier;
        }

        /// <summary>
        /// Changes the sprite of this animation every so seconds.
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator UpdateEveryXSeconds()
        {
            while (gameObject.activeSelf)
            {
                if (_index >= _sprites.Length)
                {
                    _index = 0;
                }

                image.sprite = _sprites[_index];
                _index++;

                yield return new WaitForSeconds(secondsBetweenSprite * _speedMultiplier);
            }
        }
    }
}
