using System;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace UI.PlayerVirusMeter
{
	public sealed class PlayerVirusMeterUI : MonoBehaviour, IPlayerVirusMeterUI
	{
		[SerializeField] private Image _barImage;
		[Tooltip("Lower index is shorter.")]
		[SerializeField] private Sprite[] _barSprites;

		public void SetVirusPercentage(float percentage)
		{
			if (percentage is < 0 or > 1.1f)
			{
				throw new ArgumentException("Virus meter percentage cannot be outside of 0 and 1.");
			}

			UpdateBarLength(percentage);
		}

		/// <summary>
		/// Updates the UI sprite given the percentage.
		/// </summary>
		/// <param name="percentage">The value that determines the length of bar</param>
		/// <remarks>Assume that the percentage is between 0 and 1 inclusive.</remarks>
		private void UpdateBarLength(float percentage)
		{
			int indexToAccess = Mathf.Min(Mathf.FloorToInt(percentage * _barSprites.Length),
				_barSprites.Length - 1);
			_barImage.sprite = _barSprites[indexToAccess];
		}
	}
}