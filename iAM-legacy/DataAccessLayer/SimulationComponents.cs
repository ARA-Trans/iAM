using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Utility.ExceptionHandling;
using DataAccessLayer.DTOs;


namespace DataAccessLayer
{
    /// <summary>
    /// Throw-away testing class...fills UI elements for testing.
    /// </summary>
    public class SimulationComponents
    {

        /// <summary>
        /// Used to extract the attribute name from a string starting a input index (usually 0)
        /// </summary>
        /// <param name="whereClause">The clause to look for [attribute] for</param>
        /// <param name="startIndex">Start index to search for attribute</param>
        /// <returns>Attribute name</returns>
        public static string FindAttribute(string whereClause,int startIndex)
        {
            int begin = whereClause.IndexOf('[',startIndex);
            int end = whereClause.IndexOf(']');
            if (begin == -1 || end == -1 || end < begin) return null;
            return whereClause.Substring(begin + 1, end-begin-1);
        }
    }
}
