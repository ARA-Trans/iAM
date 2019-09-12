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
            PriorityFunds = priority.PRIORITYFUNDS.Any()
                ? priority.PRIORITYFUNDS.Select(pf => new PriorityFundModel(pf)).ToList()
                : new List<PriorityFundModel>();
        }

        public void UpdatePriority(PriorityEntity priority)
        {
            priority.PRIORITYLEVEL = PriorityLevel;
            priority.YEARS = Year;
            priority.CRITERIA = Criteria;
        }
    }
}