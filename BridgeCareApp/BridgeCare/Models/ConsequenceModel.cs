using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class ConsequenceModel : CrudModel
    {
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string Change { get; set; }
        public string Criteria { get; set; }
        public string Equation { get; set; }
        public bool? IsFunction { get; set; }

        public ConsequenceModel() { }

        public ConsequenceModel(ConsequencesEntity entity)
        {
            Id = entity.CONSEQUENCEID.ToString();
            Attribute = entity.ATTRIBUTE_;
            Change = entity.CHANGE_;
            Criteria = entity.CRITERIA;
            Equation = entity.EQUATION;
            IsFunction = entity.ISFUNCTION ?? false;
        }

        public void UpdateConsequence(ConsequencesEntity entity)
        {
            entity.ATTRIBUTE_ = Attribute;
            entity.CHANGE_ = Change;
            entity.CRITERIA = Criteria;
            entity.EQUATION = Equation;
            entity.ISFUNCTION = IsFunction ?? false;
        }
    }
}