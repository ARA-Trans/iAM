using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Consequence for applying a treatment.
    /// </summary>
    public class ConsequenceStore
    {
        string _attribute;
        string _change;
        string _equation;
        string _criteria;

        /// <summary>
        /// Roadcare pretty name.
        /// </summary>
        public string Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }

        /// <summary>
        /// Simple change of value.   +,% or absolute
        /// </summary>
        public string Change
        {
            get { return _change; }
            set { _change = value; }
        }

        /// <summary>
        /// Complex change (multiple attributes [])
        /// </summary>
        public string Equation
        {
            get { return _equation; }
            set { _equation = value; }
        }

        /// <summary>
        /// Criteria when to apply the change.  For OMS decision engine this is usually empty/null.
        /// </summary>
        public string Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        public ConsequenceStore()
        {
        }

        public ConsequenceStore(string attribute, string change, string equation, string criteria)
        {
            _attribute = attribute;
            _change = change;
            _equation = equation;
            _criteria = criteria;
        }

        public override string ToString()
        {
            string output = this.Attribute;
            if (!string.IsNullOrWhiteSpace(_equation)) output += " (" + _equation + ")";
            if (!string.IsNullOrWhiteSpace(_change)) output += " (" + _change + ")";
            return output;
        }
    }
}
