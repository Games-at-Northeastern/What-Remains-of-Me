using System;

namespace UI.PlayerVirusMeter
{
	/// <summary>
	/// Allows the display of the player's virus value, both current and delayed.
	/// </summary>
	public interface IPlayerVirusMeterUI
	{
		/// <summary>
		/// Sets the current virus percentage of this UI.
		/// </summary>
		/// <param name="percentage">A fraction between 0 and 1 inclusive</param>
		/// <exception cref="ArgumentException">Thrown when the percentage is not within 0 and 1 inclusive.</exception>
		void SetCurrVirusPercentage(float percentage);
		
		/// <summary>
		/// Sets the delayed virus percentage of this UI.
		/// </summary>
		/// <param name="percentage">A fraction between 0 and 1 inclusive</param>
		/// <exception cref="ArgumentException">Thrown when the percentage is not within 0 and 1 inclusive.</exception>
		void SetDelayedVirusPercentage(float percentage);
		
		/// <summary>
		/// Sets the bar holder lights percentage of this UI.
		/// </summary>
		/// <param name="percentage">A fraction between 0 and 1 inclusive</param>
		/// <exception cref="ArgumentException">Thrown when the percentage is not within 0 and 1 inclusive.</exception>
		void SetBarHolderLightsPercentage(float percentage);
	}
}