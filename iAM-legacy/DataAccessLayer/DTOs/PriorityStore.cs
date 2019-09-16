using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class PriorityStore
    {
        string _priorityID;
        int _priorityLevel;
        string _criteria;
        string _years;
        List<PriorityFundStore> _priorityFunds;

        public string PriorityID
        {
            get { return _priorityID; }
            set { _priorityID = value; }
        }

        public int PriorityLevel
        {
            get { return _priorityLevel; }
            set { _priorityLevel = value; }
        }

        public string Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        public string Years
        {
            get { return _years; }
            set { _years = value; }
        }

        public List<PriorityFundStore> PriorityFunds
        {
            get { return _priorityFunds; }
            set { _priorityFunds = value; }
        }


        public PriorityStore()
        {
        }

        public PriorityStore(string priorityID, int priorityLevel, string criteria,string years, List<PriorityFundStore> priorityFunds)
        {
            _priorityID = priorityID;
            _priorityLevel = priorityLevel;
            _criteria = criteria;
            _years = years;
            _priorityFunds = priorityFunds;
        }
    }
}
