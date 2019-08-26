using BridgeCare.EntityClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Models
{
    public class PriorityModel : CrudModel
    {
        public string Id { get; set; }
        public int PriorityLevel { get; set; }
        public int Year { get; set; }
        public string Criteria { get; set; }
        public List<PriorityFundModel> PriorityFunds { get; set; }

        public PriorityModel() { }

        public PriorityModel(PriorityEntity priority)
        {
            Id = priority.PRIORITYID.ToString();
            PriorityLevel = priority.PRIORITYLEVEL ?? 1;
            Year = priority.YEARS ?? DateTime.Now.Year;
            Criteria = priority.CRITERIA;
            PriorityFunds = new List<PriorityFundModel>();
            if (priority.PRIORITYFUNDS.Any())
            {
                priority.PRIORITYFUNDS.ToList().ForEach(priorityFund => PriorityFunds.Add(new PriorityFundModel(priorityFund)));
            }
        }

        public void UpdatePriority(PriorityEntity priority)
        {
            priority.PRIORITYLEVEL = PriorityLevel;
            priority.YEARS = Year;
            priority.CRITERIA = Criteria;
        }
    }
}