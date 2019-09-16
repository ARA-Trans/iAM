using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class CommittedStore
    {
        string _sectionID;
        int _years;
        string _treatmentName;
        int _yearsSame;
        int _yearsAny;
        string _budget;
        float _cost = 0;
        bool _omsIsRepeat;
        bool _omsIsExclusive;
        bool _omsIsNotAllowed;



        public string SectionID
        {
            get { return _sectionID; }
            set { _sectionID = value; }
        }

        public int Years
        {
            get { return _years; }
            set { _years = value; }
        }

        public string TreatmentName
        {
            get { return _treatmentName; }
            set { _treatmentName = value; }
        }

        public int YearsSame
        {
            get { return _yearsSame; }
            set { _yearsSame = value; }
        }

        public int YearsAny
        {
            get { return _yearsAny; }
            set { _yearsAny = value; }
        }

        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public float Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public bool OMSIsRepeat
        {
            get { return _omsIsRepeat; }
            set { _omsIsRepeat = value; }
        }

        public bool OMSIsExclusive
        {
            get { return _omsIsExclusive; }
            set { _omsIsExclusive = value; }
        }

        public bool OMSIsNotAllowed
        {
            get { return _omsIsNotAllowed; }
            set { _omsIsNotAllowed = value; }
        }

        public CommittedStore()
        {
        }
    }
}
