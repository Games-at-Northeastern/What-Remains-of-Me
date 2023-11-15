namespace Levels.Objects.Platform
{
    /// <summary>
    /// An enumeration for the different wrap types for moveing objects.
    /// <para>[Enum Value] = [Point Ordering]</para>
    /// <para>Wrap = 1 > 2 > 3 > 1 > 2 > 3</para>
    /// <para>Pingpong = 1 > 2 > 3 > 2 > 1</para>
    /// <para>OneWay = 1 > 2 > 3 STOP</para>
    /// <para>None = 1 STOP</para>
    /// </summary>
    public enum LoopType
    {
        Wrap,     // 1 > 2 > 3 > 1 > 2 > 3
        Pingpong, // 1 > 2 > 3 > 2 > 1
        OneWay,   // 1 > 2 > 3 STOP
        None      // 1 STOP
    }

    /// <summary>
    /// A Utility class for abstracting common logic with LoopTypes.
    /// </summary>
    public static class LoopTypeUtility
    {
        /// <summary>
        /// Based on the given looptype, current index, max length of list, if the object is moving right, and if the object
        /// has already completed the path, returns the next index in the path.
        ///<para>This was pulled straight out of Platform.cs with little to no modification, so there are some refs thrown
        ///in there. Abstraction would be nice, but that's a bit overkill for now.</para>
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="maxLength"></param>
        /// <param name="isMovingRight"></param>
        /// <param name="completed"></param>
        /// <param name="loopType"></param>
        /// <returns></returns>
        public static int GetNextIndex(int currentIndex, int maxLength, ref bool isMovingRight, ref bool completed, LoopType loopType)
        {
            int dir = isMovingRight ? 1 : -1;

            switch (loopType) // could replace this with an enhanced switch, but nah.
            {
                case LoopType.Wrap: // wraps around if out of bounds
                    return currentIndex + dir < 0 ? maxLength - 1 : (currentIndex + dir) % maxLength;

                case LoopType.Pingpong: // bounces back if out of bounds
                    int next = currentIndex + dir;

                    if (next < 0) // hit left side
                    {
                        isMovingRight = true;
                        return currentIndex + 1;
                    }
                    else if (next >= maxLength) // hit right side
                    {
                        isMovingRight = false;
                        return currentIndex - 1;
                    }

                    return next;

                case LoopType.OneWay: // does not move after hitting opposite side
                    int nextIndex = currentIndex + dir;

                    if (nextIndex < 0 || nextIndex >= maxLength)
                    {
                        completed = true;
                        return currentIndex;
                    }

                    return nextIndex;

                default: // None
                    completed = true;
                    return currentIndex;
            }
        }
    } 
}
