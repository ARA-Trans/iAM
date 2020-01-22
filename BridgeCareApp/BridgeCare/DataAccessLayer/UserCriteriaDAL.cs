using System.Collections.Generic;
using System.Data;
using System.Linq;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.DataAccessLayer
{
    public class UserCriteriaDAL : IUserCriteria
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserCriteriaDAL));

        /// <summary>
        /// Gets the UserCriteria for all users.
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>UserCriteriaModel List</returns>
        public List<UserCriteriaModel> GetAllUserCriteria(BridgeCareContext db) =>
            db.UserCriteria.ToList().Select(criteria => new UserCriteriaModel(criteria)).ToList();

        /// <summary>
        /// Gets the UserCriteria of the specified user.
        /// If a user does not have any criteria,
        /// a default setting will be created for them based on their role.
        /// </summary>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="userInformation">UserInformationModel</param>
        /// <returns>UserCriteriaModel</returns>
        public UserCriteriaModel GetOwnUserCriteria(BridgeCareContext db, UserInformationModel userInformation)
        {
            if (!db.UserCriteria.Any(criteria => criteria.USERNAME == userInformation.Name))
            {
                log.Info($"User '{userInformation.Name}' has logged in for the first time.");
                var newUserCriteria = new UserCriteriaEntity(GenerateDefaultUserCriteria(userInformation));
                db.UserCriteria.Add(newUserCriteria);
                db.SaveChanges();
                return new UserCriteriaModel(newUserCriteria);
            }
            var userCriteria = db.UserCriteria.Single(criteria => criteria.USERNAME == userInformation.Name);
            return new UserCriteriaModel(userCriteria);
        }

        public void SaveUserCriteria(UserCriteriaModel model, BridgeCareContext db)
        {
            if (!db.UserCriteria.Any(criteria => criteria.USERNAME == model.Username))
            {
                log.Error($"No user found with username {model.Username}.");
                throw new RowNotInTableException($"No user found with username {model.Username}.");
            }
            var userCriteria = db.UserCriteria.Single(criteria => criteria.USERNAME == model.Username);
            model.UpdateUserCriteria(userCriteria);
            db.SaveChanges();
        }

        /// <summary>
        /// Creates a default UserCriteriaModel for a new user, based on their role.
        /// Administrators have full access by default.
        /// All other users have no access by default.
        /// </summary>
        /// <param name="userInformation">UserInformationModel</param>
        /// <returns>UserCriteriaModel</returns>
        private UserCriteriaModel GenerateDefaultUserCriteria(UserInformationModel userInformation) =>
            new UserCriteriaModel
            {
                Username = userInformation.Name,
                Criteria = null,
                HasCriteria = false,
                HasAccess = userInformation.Role == Role.ADMINISTRATOR
            };
    }
}
