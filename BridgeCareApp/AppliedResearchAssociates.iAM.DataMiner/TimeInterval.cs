using System;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    /// <summary>
    /// Base class for determining time as an integer number, such as years, since a given origin date.
    /// </summary>
    /// <remarks>It would be nice to use the native TimeSpan object, but days are the largest interval it can handle</remarks>
    public abstract class TimeInterval
    {
        /// <summary>
        /// Builds a TimeInterval using a given origin date
        /// </summary>
        /// <param name="originDate">Time where interval is 0</param>
        public TimeInterval(DateTime originDate)
        {
            Origin = originDate;
        }

        /// <summary>
        /// Date where time interval would be 0.
        /// </summary>
        public DateTime Origin { get; }

        /// <summary>
        /// Calculates the number of intervals since a provided origin date
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns>Number of intervals since the origin date.  Can be negative.</returns>
        public abstract int CalculateInterval(DateTime TimeStamp);
    }
}
