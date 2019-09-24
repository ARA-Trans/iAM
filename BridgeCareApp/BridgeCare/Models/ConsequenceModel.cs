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

        public ConsequenceModel(ConsequencesEntity consequencesEntity)
        {
            Id = consequencesEntity.CONSEQUENCEID.ToString();
            Attribute = consequencesEntity.ATTRIBUTE_;
            Change = consequencesEntity.CHANGE_;
            Criteria = consequencesEntity.CRITERIA;
            Equation = consequencesEntity.EQUATION;
            IsFunction = consequencesEntity.ISFUNCTION ?? false;
        }

        public void UpdateConsequence(ConsequencesEntity consequencesEntity)
        {
            consequencesEntity.ATTRIBUTE_ = Attribute;
            consequencesEntity.CHANGE_ = Change;
            consequencesEntity.CRITERIA = Criteria;
            consequencesEntity.EQUATION = Equation;
            consequencesEntity.ISFUNCTION = IsFunction ?? false;
        }
    }
}