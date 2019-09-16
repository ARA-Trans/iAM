using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class TargetStore
    {
        string _id;
        string _name;
        double _targetOCI;
        string _criteria;
        string _year;
        string _isOMSPriority;
        ErrorStore _error;


        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double TargetOCI
        {
            get { return _targetOCI; }
            set { _targetOCI = value; }
        }

        public string Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        public string Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public string IsOMSPriority
        {
            get { return _isOMSPriority; }
            set { _isOMSPriority = value; }
        }

        public ErrorStore Error
        {
            get { return _error; }
            set { _error = value; }
        }


        public TargetStore()
        {

        }
    }
}
