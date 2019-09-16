using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Stores costs for Treatment/Activities
    /// </summary>
    public class CostStore
    {
        string _equation;
        string _criteria;
        
        /// <summary>
        /// Cost calculation equation
        /// </summary>
        public string Equation
        {
            get { return _equation; }
            set { _equation = value; }
        }

        /// <summary>
        /// Criteria when to apply
        /// </summary>
        public string Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        public CostStore()
        {
        }

        public CostStore(string equation, string criteria)
        {
            _equation = equation;
            _criteria = criteria;
        }
    }
}
