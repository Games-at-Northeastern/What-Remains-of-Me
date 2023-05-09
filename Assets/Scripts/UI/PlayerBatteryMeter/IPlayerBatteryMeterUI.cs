namespace UI.PlayerBatteryMeter
{
    /// <summary>
    /// Allow the display of the player's battery value.
    /// </summary>
    public interface IPlayerBatteryMeterUI
    {
		/// <summary>
		/// Sets the current battery percentage of this UI.
		/// </summary>
		/// <param name="percentage">A fraction between 0 and 1 inclusive</param>
		/// <exception cref="ArgumentException">Thrown when the percentage is not within 0 and 1 inclusive.</exception>
        void SetCurrBatteryPercentage(float percentage);

		/// <summary>
		/// Sets the current virus percentage of this UI.
		/// </summary>
		/// <param name="percentage">A fraction between 0 and 1 inclusive</param>
		/// <exception cref="ArgumentException">Thrown when the percentage is not within 0 and 1 inclusive.</exception>
        void SetCurrVirusPercentage(float percentage);
    }
}

