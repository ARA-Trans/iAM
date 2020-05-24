using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    /// <summary>
    /// Represents a value that changes over time.
    /// </summary>
    /// <typeparam name="T">Base type of the value</typeparam>
    public class TemporalValue<T>
    {
        /// <summary>
        /// Repository of historical values
        /// </summary>
        private Dictionary<DateTime, T> dataValues;

        /// <summary>
        /// Time interval used when retirving data for a given interval
        /// </summary>
        private TimeInterval timeInterval;

        /// <summary>
        /// Rule used when multiple datapoints are available for a given interval
        /// </summary>
        private AggregationRule<T> aggregationRule;

        /// <summary>
        /// Gets the most recent value prior to a date
        /// </summary>
        public virtual T GetData(DateTime timeStamp)
        {
            // TODO: Determine data value if it exists.
            return default(T);
        }

        /// <summary>
        /// Get the value for a given time interval
        /// </summary>
        /// <param name="interval">Time interval to retrieve</param>
        /// <example>If the TimeInterval for this object represents a year, this would be the year</example>
        /// <remarks>A query in this routine may return multiple values that would need to be integrated</remarks>
        public virtual T GetData(int interval)
        {
            // TODO: Determine single data value if it exists
            return default(T);
        }

        /// <summary>
        /// Determines if two temporal values have data that use the same dates
        /// </summary>
        /// <param name="comparisonValue">The temporal value that should be compared to this value</param>
        /// <returns>True if both objects have values for the same dates</returns>
        protected bool IsConsistentDate(TemporalValue<T> comparisonValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if two temporal values have data that use the same intervals
        /// </summary>
        /// <param name="comparisonValue">The temporal value that should be compared to this value</param>
        /// <returns>True if both objects have values for the same intervals</returns>
        protected bool IsConsistentInterval(TemporalValue<T> comparisonValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lists the dates in dataVlaues for use by the IsConsistentDate function
        /// </summary>
        /// <returns>Collection of dates</returns>
        protected IEnumerable<DateTime> GetDates()
        {
            List<DateTime> keyList = new List<DateTime>();
            foreach (var item in dataValues)
            {
                keyList.Add(item.Key);
            }
            return keyList;
        }

        /// <summary>
        /// Lists the available intervals for use by the IsConsistentInterval function
        /// </summary>
        /// <returns>List of intervals</returns>
        protected IEnumerable<int> GetIntervals()
        {
            throw new NotImplementedException();
        }
    }
}
