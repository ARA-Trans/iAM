using System;
using System.ComponentModel.Design;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    /// <summary>
    /// Represents time intervals in years
    /// </summary>
    public class TimeIntervalYear : TimeInterval
    {
        /// <summary>
        /// Create a year-bsed time interval assuming that the year starts on January 1.
        /// </summary>
        /// <param name="originDate">Date where time interval is 0</param>
        public TimeIntervalYear(DateTime originDate) : base(originDate)
        {
            StartDay = 1;
            StartMonth = 1;
        }

        /// <summary>
        /// Create a year-based time interval using a user defined start date for the year
        /// </summary>
        /// <param name="originDate">Date where time interval is 0</param>
        /// <param name="startMonth">Numeric month for start of year</param>
        /// <param name="startDay">Numeric day for end of day</param>
        public TimeIntervalYear(DateTime originDate, int startMonth, int startDay) : base(originDate)
        {
            // Check for a valid month
            if (startMonth > 12 || startMonth < 1) throw new ArgumentException("Month must be a value between 1 ans 12");

            // Check for a valid day (i.e., day can never be 32 and sometimes it cannot be 31)
            switch (startMonth)
            {
                case 2:
                    if (startDay > 28) throw new ArgumentException("February can not have more than 28 days");
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    if (startDay > 30) throw new ArgumentException("Specified month can not have more than 30 days");
                    break;
                default:
                    break;
            }

            StartDay = startDay;
            StartMonth = startMonth;
        }

        /// <summary>
        /// Numeric month to use for when a year starts
        /// </summary>
        private int StartMonth;

        /// <summary>
        /// Numeric day to use for when a year starts
        /// </summary>
        private int StartDay;

        public override int CalculateInterval(DateTime TimeStamp)
        {
            // This should be a simple subtract one year from another year.  However, there
            // is the case where one date is before the starting date defined by StartMonth
            // and StartYear.  Calculate an adjustment factor based on that possibilty.
            int adjustmentFactor = 0;
            DateTime OriginDate = new DateTime(Origin.Year, StartMonth, StartDay);
            DateTime DataDate = new DateTime(TimeStamp.Year, StartMonth, StartDay);

            // 1. Check for the case where the origin date is prior to the starting date
            // and the timestamp is after the starting date.
            if (Origin < OriginDate && TimeStamp > DataDate) adjustmentFactor = 1;

            // 2. Check for the case where the origin date is after the starting date and the
            // timestamp is before the origin date.
            if (Origin > OriginDate && TimeStamp < DataDate) adjustmentFactor = -1;

            // 3. Return the date difference
            return TimeStamp.Year - Origin.Year + adjustmentFactor;
        }
    }
}
