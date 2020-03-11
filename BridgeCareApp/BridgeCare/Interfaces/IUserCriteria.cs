using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IUserCriteria
    {
        UserCriteriaModel GetOwnUserCriteria(BridgeCareContext db, UserInformationModel userInformation);
        List<UserCriteriaModel> GetAllUserCriteria(BridgeCareContext db);
        void SaveUserCriteria(UserCriteriaModel model, BridgeCareContext db);
    }
}
