using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    class ReportOMS
    {
        string _sectionID;
        int _year;
        string _treatment;
        int _yearsAny;
        int _yearsSame;
        string _budget;
        double _cost;
        string _changeHash;
        double _area;
        int _resultType;
        int _commitOrder;
        Dictionary<string, string> _attributeChange;

        public string SectionID
        {
            get { return _sectionID; }
            set { _sectionID = value; }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public string Treatment
        {
            get { return _treatment; }
            set { _treatment = value; }
        }

        public int YearsAny
        {
            get { return _yearsAny; }
            set { _yearsAny = value; }
        }

        public int YearsSame
        {
            get { return _yearsSame; }
            set { _yearsSame = value; }
        }

        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public double Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public string ChangeHash
        {
            set { 
                 _changeHash = value;
                 _attributeChange = new Dictionary<string, string>();
                 string[] rows = _changeHash.Split('\n');
                 foreach (string row in rows)
                 {
                     if (!string.IsNullOrWhiteSpace(row))
                     {
                         string[] values = row.Split('\t');
                         _attributeChange.Add(values[0], values[1]);
                     }
                }
            }
        }

        public double Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int ResultType
        {
            get { return _resultType; }
            set { _resultType = value; }
        }

        public Dictionary<string, string> AttributeChange
        {
            get { return _attributeChange; }
            set { _attributeChange = value; }
        }

        public int CommitOrder
        {
            get { return _commitOrder; }
            set { _commitOrder = value; }
        }


        public ReportOMS()
        {
        }
    }
}
