using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Stores information from OMS concerning an activity
    /// </summary>
    public class OMSActivityStore
    {
        string _activityOID;
        string _activityName;
        OMSCostStore _omsCost = null;
        List<OMSConsequenceStore> _consequenceList = null;

        public string ActivityOID
        {
            get { return _activityOID; }
            set { _activityOID = value; }
        }

        public string ActivityName
        {
            get { return _activityName; }
            set { _activityName = value; }
        }

        public OMSCostStore OmsCost
        {
            get { return _omsCost; }
            set { _omsCost = value; }
        }

       
        public List<OMSConsequenceStore> ConsequenceList
        {
            get { return _consequenceList; }
            set { _consequenceList = value; }
        }


        public OMSActivityStore()
        {
        }
        
        
        public OMSActivityStore(string activityOID, string activityName, double cost, string unit)
        {
            _activityOID = activityOID;
            _activityName = activityName;
            _omsCost = new OMSCostStore(unit, cost);
            _consequenceList = new List<OMSConsequenceStore>();
        }

        public override string ToString()
        {
            string consequences = "(";

            foreach (OMSConsequenceStore consequence in _consequenceList)
            {
                consequences += consequence.ToString();
            }
            consequences += ")";
            return _activityName + consequences;
        }

    }
}
