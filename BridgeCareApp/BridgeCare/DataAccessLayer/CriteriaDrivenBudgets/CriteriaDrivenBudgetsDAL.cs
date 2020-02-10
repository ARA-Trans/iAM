using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using BridgeCare.Interfaces.CriteriaDrivenBudgets;
using BridgeCare.Models.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCare.DataAccessLayer.CriteriaDrivenBudgets
{
    public class CriteriaDrivenBudgetsDAL : ICriteriaDrivenBudgets
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CriteriaDrivenBudgetsDAL));
        /// <summary>
        /// Fetches a simulation's criteria driven budgets
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>CriteriaDrivenBudgetsModel list</returns>
        private List<CriteriaDrivenBudgetsModel> GetCriteriaDrivenBudgets(int id, BridgeCareContext db)
        {
            if (db.CriteriaDrivenBudgets.Any(cdb => cdb.SIMULATIONID == id))
                return db.CriteriaDrivenBudgets.AsNoTracking()
                    .Where(cbd => cbd.SIMULATIONID == id)
                    .ToList()
                    .Select(cbd => new CriteriaDrivenBudgetsModel(cbd))
                    .ToList();

            return new List<CriteriaDrivenBudgetsModel>();
        }

        /// <summary>
        /// Fetches a simulation's criteria driven budgets if the scenario belongs to the user
        /// Throws RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="id">Simulation id</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>CriteriaDrivenBudgetsModel list</returns>
        public List<CriteriaDrivenBudgetsModel> GetOwnedCriteriaDrivenBudgets(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.UserCanRead(username) && s.SIMULATIONID == id))
            {
                log.Warn($"User {username} is not authorized to view scenario {id}.");
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's criteria driven budgets.");
            }
            return GetCriteriaDrivenBudgets(id, db);
        }

        /// <summary>
        /// Fetches a simulation's criteria driven budgets
        /// Throws RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="id">Simulation id</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>CriteriaDrivenBudgetsModel list</returns>
        public List<CriteriaDrivenBudgetsModel> GetAnyCriteriaDrivenBudgets(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
            {
                log.Warn($"No scenario found with {id}");
                throw new RowNotInTableException($"No scenario found with {id}");
            }
            return GetCriteriaDrivenBudgets(id, db);
        }

        /// <summary>
        /// Executes an insert/delete operation on the criteria driven budgets table
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="models">CriteriaDrivenBudgetsModel list</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>string Task</returns>
        private Task<string> SaveCriteriaDrivenBudgets(int id, List<CriteriaDrivenBudgetsModel> models, BridgeCareContext db)
        {
            try
            {
                if (db.CriteriaDrivenBudgets.Any(cdb => cdb.SIMULATIONID == id))
                    db.CriteriaDrivenBudgets
                        .RemoveRange(db.CriteriaDrivenBudgets
                            .Where(budgets => budgets.SIMULATIONID == id)
                            .ToList()
                        );

                db.CriteriaDrivenBudgets
                    .AddRange(models
                        .Select(criteriaModel => new CriteriaDrivenBudgetsEntity(id, criteriaModel))
                        .ToList()
                    );

                db.SaveChanges();

                return Task.FromResult("Saved criteria driven budgets");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Task.FromResult($"Failed to save criteria driven budgets::{ex.Message}");
            }
        }

        public Task<string> SaveOwnedCriteriaDrivenBudgets(int id, List<CriteriaDrivenBudgetsModel> models, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.UserCanModify(username) && s.SIMULATIONID == id))
            {
                log.Warn($"User {username} is not authorized to modify scenario {id}.");
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's criteria driven budgets.");
            }
            return SaveCriteriaDrivenBudgets(id, models, db);
        }

        public Task<string> SaveAnyCriteriaDrivenBudgets(int id, List<CriteriaDrivenBudgetsModel> models, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
            {
                log.Warn($"No scenario found with {id}");
                throw new RowNotInTableException($"No scenario found with {id}");
            }
            return SaveCriteriaDrivenBudgets(id, models, db);
        }
    }
}
