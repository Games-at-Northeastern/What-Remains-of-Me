using System;

namespace UI
{
	/// <summary>
	/// Allows the display of the player's virus value.
	/// </summary>
	public interface IPlayerVirusMeterUI
	{
		/// <summary>
		/// Sets the virus percentage of this UI.
		/// </summary>
		/// <param name="percentage">A fraction between 0 and 1 inclusive</param>
		/// <exception cref="ArgumentException">Thrown when the percentange is not within 0 and 1 inclusive.</exception>
		void SetVirusPercentage(float percentage);
	}
}