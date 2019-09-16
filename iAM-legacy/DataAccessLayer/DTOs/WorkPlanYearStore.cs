using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class WorkPlanYearStore
    {
        string _spent;
        string _budget;
        string _targetOCI;
        string _currentOCI;
        string _year;
        string _index;
        string _numberTreatments;
        
        public string Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public string Spent
        {
            get { return _spent; }
            set { _spent = value; }
        }

        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public string TargetOCI
        {
            get { return _targetOCI; }
            set { _targetOCI = value; }
        }

        public string CurrentOCI
        {
            get { return _currentOCI; }
            set { _currentOCI = value; }
        }

        public string Year
        {
            get { return _year; }
            set { _year = value; }
        }

       public string NumberTreatments
        {
            get { return _numberTreatments; }
            set { _numberTreatments = value; }
        }


        public WorkPlanYearStore()
        {

        }
    }
}
