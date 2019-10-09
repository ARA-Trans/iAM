using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace BridgeCare.Models
{
  public class UserInformationModel
  {
    public string Name { get; set; }
    public string Id { get; set; }

    public UserInformationModel() { }

    public UserInformationModel(WindowsIdentity identity)
    {
        Name = identity.Name;
        Id = identity.User.ToString();
    }
  }
}