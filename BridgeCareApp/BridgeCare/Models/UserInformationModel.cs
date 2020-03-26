namespace BridgeCare.Models
{
    public class UserInformationModel
    {
        public string Name { get; }
        public string Role { get; }
        public string Email { get; }

        public UserInformationModel(string name, string role, string email)
        {
            Name = name;
            Role = role;
            Email = email;
        }
    }
}
