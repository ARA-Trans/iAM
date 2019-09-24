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

        public CostModel(CostsEntity costsEntity)
        {
            Id = costsEntity.COSTID.ToString();
            Equation = costsEntity.COST_;
            Criteria = costsEntity.CRITERIA;
            IsFunction = costsEntity.ISFUNCTION ?? false;
        }

        public void UpdateCost(CostsEntity costsEntity)
        {
            costsEntity.COST_ = Equation;
            costsEntity.CRITERIA = Criteria;
            costsEntity.ISFUNCTION = IsFunction ?? false;
        }
    }
}