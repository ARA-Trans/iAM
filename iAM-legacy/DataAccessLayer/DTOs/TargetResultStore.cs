using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class TargetResultStore
    {
        int _ID;
        int _targetID;
        string _years;
        double _targetMet;
        double _area;
        bool _isDeficient;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int TargetID
        {
            get { return _targetID; }
            set { _targetID = value; }
        }

        public string Years
        {
            get { return _years; }
            set { _years = value; }
        }

        public double TargetMet
        {
            get { return _targetMet; }
            set { _targetMet = value; }
        }

        public double Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public bool IsDeficient
        {
            get { return _isDeficient; }
            set { _isDeficient = value; }
        }

        public TargetResultStore()
        {
        }
    }
}
