﻿using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class PriorityFundModel : CrudModel
    {
        public int PriorityId { get; set; }
        public int Id { get; set; }
        public string Budget { get; set; }
        public double? Funding { get; set; }

        public PriorityFundModel() { }

        public PriorityFundModel(PriorityFundEntity priorityFund)
        {
            PriorityId = priorityFund.PRIORITYID;
            Id = priorityFund.PRIORITYFUNDID;
            Budget = priorityFund.BUDGET ?? "";
            Funding = priorityFund.FUNDING ?? 0;
        }

        public void UpdatePriorityFund(PriorityFundEntity priorityFund)
        {
            priorityFund.BUDGET = Budget;
            priorityFund.FUNDING = Funding;
        }
    }
}