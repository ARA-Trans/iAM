namespace BridgeCare.Models
{
    public class UserInformationModel
    {
        public string Name { get; }
        public string Role { get; }

        public UserInformationModel(string name, string role)
        {
            Name = name;
            Role = role;
        }
    }
}