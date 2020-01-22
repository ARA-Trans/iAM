using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class UserCriteriaModel
    {
        public string Username { get; set; }
        public string Criteria { get; set; }
        public bool HasCriteria { get; set; }
        public bool HasAccess { get; set; }

        public UserCriteriaModel() { }

        public UserCriteriaModel(UserCriteriaEntity entity)
        {
            Username = entity.USERNAME;
            Criteria = entity.CRITERIA;
            HasAccess = entity.HAS_ACCESS;
            HasCriteria = entity.CRITERIA != null;
        }

        public void UpdateUserCriteria(UserCriteriaEntity entity)
        {
            entity.USERNAME = Username;
            entity.CRITERIA = Criteria;
            entity.HAS_ACCESS = HasAccess;
        }
    }
}
