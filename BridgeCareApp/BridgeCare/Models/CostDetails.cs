namespace BridgeCare.Models
{
    public class CostDetails
    {
        public double Cost { get; set; }
        public int Years { get; set; }
        public string Budget { get; set; }

        public CostDetails() { }

        public CostDetails(BudgetModel model)
        {
            Cost = model.Cost_ ?? 0;
            Years = model.Years;
            Budget = model.Budget;
        }
    }
}