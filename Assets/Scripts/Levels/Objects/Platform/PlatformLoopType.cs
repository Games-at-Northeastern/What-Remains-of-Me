namespace Levels.Objects.Platform
{
    /// <summary>
    /// An enumeration for the different wrap types for platform movements.
    /// <para>[Enum Value] = [Point Ordering]</para>
    /// <para>Wrap = 1 > 2 > 3 > 1 > 2 > 3</para>
    /// <para>Pingpong = 1 > 2 > 3 > 2 > 1</para>
    /// <para>OneWay = 1 > 2 > 3 STOP</para>
    /// <para>None = 1 STOP</para>
    /// </summary>
    public enum PlatformLoopType
    {
        Wrap,     // 1 > 2 > 3 > 1 > 2 > 3
        Pingpong, // 1 > 2 > 3 > 2 > 1
        OneWay,   // 1 > 2 > 3 STOP
        None      // 1 STOP
    }
}
