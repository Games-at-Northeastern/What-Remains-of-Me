using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;

namespace UI.PlayerVirusMeter
{
	public sealed class PlayerVirusMeterUI : MonoBehaviour, IPlayerVirusMeterUI
	{
		[Header("Dependency Injection")]
		[SerializeField] private Image _currBarImage;
		[SerializeField] private Image _delayedBarImage;
		[SerializeField] private Image _barHolderLightImage;
		
		[SerializeField] private Animator _animator;
		
		[Header("Colors")]
		[Tooltip("Lower index is lower virus.")]
		[SerializeField] private Color32[] _colors;
		
		[Tooltip("Lower index is shorter.")]
		[SerializeField] private Sprite[] _barSprites;

		private static readonly int Virus = Animator.StringToHash("Virus");

		public void SetCurrVirusPercentage(float percentage)
		{
			// 1.05f and not 1f because comparison with floats are wonky
			if (percentage is < 0 or > 1.05f)
			{
				throw new ArgumentException("Virus meter percentage cannot be outside of 0 and 1.");
			}

			UpdateBarLength(_currBarImage, percentage);
			_animator.SetFloat(Virus, percentage);
			UpdateColor(_currBarImage, percentage);
		}

		public void SetDelayedVirusPercentage(float percentage)
		{
			// 1.05f and not 1f because comparison with floats are wonky
			if (percentage is < 0 or > 1.05f)
			{
				throw new ArgumentException("Virus meter percentage cannot be outside of 0 and 1.");
			}

			UpdateBarLength(_delayedBarImage, percentage);
		}

		public void SetBarHolderLightsPercentage(float percentage)
		{
			// 1.05f and not 1f because comparison with floats are wonky
			if (percentage is < 0 or > 1.05f)
			{
				throw new ArgumentException("Virus meter percentage cannot be outside of 0 and 1.");
			}
			
			UpdateColor(_barHolderLightImage, percentage);
		}

		private void UpdateColor(Image image, float percentage)
		{
			int indexToAccess = Mathf.Min(Mathf.FloorToInt(percentage * _colors.Length),
				_colors.Length - 1);
			image.color = _colors[indexToAccess];
		}

		/// <summary>
		/// Updates the UI sprite given the percentage.
		/// </summary>
		/// <param name="barImage">The image bar to update</param>
		/// <param name="percentage">The value that determines the length of bar</param>
		/// <remarks>Assume that the percentage is between 0 and 1 inclusive.</remarks>
		private void UpdateBarLength(Image barImage, float percentage)
		{
			int indexToAccess = Mathf.Min(Mathf.FloorToInt(percentage * _barSprites.Length),
				_barSprites.Length - 1);
			barImage.sprite = _barSprites[indexToAccess];
		}
	}
}