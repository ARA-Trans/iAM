using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class TreatmentStore
    {
        string _treatmentID;
        string _treatement;
        int _beforeAny;
        int _beforeSame;
        string _budget;
        string _description;

        ActivityStore _activity;
        CostStore _cost;
        ConsequenceStore _consequence;

        public string TreatmentID
        {
            get { return _treatmentID; }
            set { _treatmentID = value; }
        }

        public string Treatement
        {
            get { return _treatement; }
            set { _treatement = value; }
        }

        public int BeforeAny
        {
            get { return _beforeAny; }
            set { _beforeAny = value; }
        }

        public int BeforeSame
        {
            get { return _beforeSame; }
            set { _beforeSame = value; }
        }

        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ActivityStore Activity
        {
            get { return _activity; }
            set { _activity = value; }
        }

        public CostStore Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public ConsequenceStore Consequence
        {
            get { return _consequence; }
            set { _consequence = value; }
        }

        public TreatmentStore()
        {
        }
    }
}
