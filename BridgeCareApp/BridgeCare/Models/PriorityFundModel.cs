using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class PriorityFundModel : CrudModel
    {
        public string Id { get; set; }
        public string Budget { get; set; }
        public double? Funding { get; set; }

        public PriorityFundModel() { }

        public PriorityFundModel(PriorityFundEntity entity)
        {
            Id = entity.PRIORITYFUNDID.ToString();
            Budget = entity.BUDGET ?? "";
            Funding = entity.FUNDING ?? 0;
        }

        public void UpdatePriorityFund(PriorityFundEntity entity)
        {
            entity.BUDGET = Budget;
            entity.FUNDING = Funding;
        }
    }
}