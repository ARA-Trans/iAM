using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class CostModel : CrudModel
    {
        public string Id { get; set; }
        public string Equation { get; set; }
        public string Criteria { get; set; }
        public bool? IsFunction { get; set; }

        public CostModel() { }

        public CostModel(CostsEntity entity)
        {
            Id = entity.COSTID.ToString();
            Equation = entity.COST_;
            Criteria = entity.CRITERIA;
            IsFunction = entity.ISFUNCTION ?? false;
        }

        public void UpdateCost(CostsEntity entity)
        {
            entity.COST_ = Equation;
            entity.CRITERIA = Criteria;
            entity.ISFUNCTION = IsFunction ?? false;
        }
    }
}