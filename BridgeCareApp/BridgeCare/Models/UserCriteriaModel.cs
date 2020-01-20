namespace BridgeCare.Models
{
    public class UserCriteriaModel
    {
        public string Username { get; set; }
        public string Criteria { get; set; }
        public bool HasCriteria { get; set; }
        public bool HasAccess { get; set; }

        public UserCriteriaModel() { }
    }
}
