namespace Levels.Objects.Platform
{
    /// <summary>
    /// Applies a movement speed 'glitch' effect to the platforms controlled by this object,
    /// dependent on the ratio of virus to clean energy in the controller (i.e. a higher virus percentage = more glitching).
    /// </summary>
    public class RandomSpeedEffector : AMovingElementVirusEffector
    {
        protected override void AffectMovingElement(MovingElement platform) => platform.SetSpeedModifier((_currentVirusPercentage * 3) + 1);
    }

}
