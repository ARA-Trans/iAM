using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    /// <summary>
    /// Performance model container class
    /// </summary>
    public class Performance
    {
        string _performanceID;
        string _attribute = "";
        string _equation = "";
        string _name = "";
        string _criteria = "";
        bool _isPiecewise = false;
        bool _isFunction = false;
        bool _isShift = false;

        public string PerformanceID
        {
            get { return _performanceID; }
            set { _performanceID = value; }
        }

        public string Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Equation
        {
            get { return _equation; }
            set { _equation = value; }
        }

        public bool IsPiecewise
        {
            get { return _isPiecewise; }
            set { _isPiecewise = value; }
        }

        public bool IsFunction
        {
            get { return _isFunction; }
            set { _isFunction = value; }
        }

        public string Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        public bool IsShift
        {
            get { return _isShift; }
            set { _isShift = value; }
        }
        
    }
}
