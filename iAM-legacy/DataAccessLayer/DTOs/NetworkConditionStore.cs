using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    public class NetworkConditionStore
    {
        /// <summary>
        /// Year for which network condition is for
        /// </summary>
        [DataMember]
        public string Year { get; set; }
        
        /// <summary>
        /// Average value of target (usually network OCI)
        /// </summary>
        [DataMember]
        public string Average { get; set; }
        
        /// <summary>
        /// Percent of assets which are deficient (usually OCI)
        /// </summary>
        [DataMember]
        public string Deficient { get; set; }


        public NetworkConditionStore()
        {
        }

        public NetworkConditionStore(string year, string average, string deficient)
        {
            this.Year = year;
            this.Average = average;
            this.Deficient = deficient;
        }
        /// <summary>
        /// Converts the string Average to a double (safe)
        /// </summary>
        /// <returns>Returns average as double or NaN</returns>
        public double GetAverage()
        {
            double average = double.NaN;
            double.TryParse(this.Average, out average);
            return average;
        }
    }
}
