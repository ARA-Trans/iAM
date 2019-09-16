using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class AssetReplyOMSConditionIndex
    {
        private string _conditionIndex;
        private double _value;
        private DateTime _inspectionDate;

        public string ConditionIndex
        {
            get { return _conditionIndex; }
            set { _conditionIndex = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public DateTime InspectionDate
        {
            get { return _inspectionDate; }
            set { _inspectionDate = value; }
        }

        public AssetReplyOMSConditionIndex(string conditionIndex, double value, DateTime inspectionDate)
        {
            _conditionIndex = conditionIndex;
            _value = value;
            _inspectionDate = inspectionDate;
        }


        public void Update(double value, DateTime inspectionDate)
        {
            if (inspectionDate > _inspectionDate)
            {
                _inspectionDate = inspectionDate;
                _value = value;
            }
        }

        public override string ToString()
        {
            return _conditionIndex + "(" + _value.ToString() + ")";
        }
    }
}
