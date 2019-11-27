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
        public int? Year { get; set; }
        public string Criteria { get; set; }
        public List<PriorityFundModel> PriorityFunds { get; set; }

        public PriorityModel() { }

        public PriorityModel(PriorityEntity entity)
        {
            Id = entity.PRIORITYID.ToString();
            PriorityLevel = entity.PRIORITYLEVEL ?? 1;
            Year = entity.YEARS;
            Criteria = entity.CRITERIA;
            PriorityFunds = entity.PRIORITYFUNDS.Any()
                ? entity.PRIORITYFUNDS.Select(pf => new PriorityFundModel(pf)).ToList()
                : new List<PriorityFundModel>();
        }

        public void UpdatePriority(PriorityEntity entity)
        {
            entity.PRIORITYLEVEL = PriorityLevel;
            entity.YEARS = Year;
            entity.CRITERIA = Criteria;
        }
    }
}