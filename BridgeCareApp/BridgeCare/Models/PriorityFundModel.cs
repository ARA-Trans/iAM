﻿using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class PriorityFundModel : CrudModel
    {
        public string Id { get; set; }
        public string Budget { get; set; }
        public double? Funding { get; set; }

        public PriorityFundModel() { }

        public PriorityFundModel(PriorityFundEntity priorityFund)
        {
            Id = priorityFund.PRIORITYFUNDID.ToString();
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