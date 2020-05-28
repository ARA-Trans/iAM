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
        /// <returns>CriteriaDrivenBudgetModel list</returns>
        private List<CriteriaDrivenBudgetModel> GetCriteriaDrivenBudgets(int id, BridgeCareContext db)
        {
            if (db.CriteriaDrivenBudgets.Any(cdb => cdb.SIMULATIONID == id))
                return db.CriteriaDrivenBudgets.AsNoTracking()
                    .Where(cbd => cbd.SIMULATIONID == id)
                    .ToList()
                    .Select(cbd => new CriteriaDrivenBudgetModel(cbd))
                    .ToList();

            return new List<CriteriaDrivenBudgetModel>();
        }

        /// <summary>
        /// Fetches a simulation's criteria driven budgets if the scenario belongs to the user
        /// Throws RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="id">Simulation id</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>CriteriaDrivenBudgetModel list</returns>
        public List<CriteriaDrivenBudgetModel> GetPermittedCriteriaDrivenBudgets(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}.");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == id).UserCanRead(username))
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's criteria driven budgets.");
            return GetCriteriaDrivenBudgets(id, db);
        }

        /// <summary>
        /// Fetches a simulation's criteria driven budgets
        /// Throws RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="id">Simulation id</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>CriteriaDrivenBudgetModel list</returns>
        public List<CriteriaDrivenBudgetModel> GetAnyCriteriaDrivenBudgets(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with {id}");
            return GetCriteriaDrivenBudgets(id, db);
        }

        /// <summary>
        /// Executes an insert/delete operation on the criteria driven budgets table
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="models">CriteriaDrivenBudgetModel list</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>string Task</returns>
        private Task<string> SaveCriteriaDrivenBudgets(int id, List<CriteriaDrivenBudgetModel> models, BridgeCareContext db)
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
                        .Select(criteriaModel => new CriteriaDrivenBudgetEntity(id, criteriaModel))
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

        public Task<string> SavePermittedCriteriaDrivenBudgets(int id, List<CriteriaDrivenBudgetModel> models, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with {id}");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == id).UserCanModify(username))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's criteria driven budgets.");
            return SaveCriteriaDrivenBudgets(id, models, db);
        }

        public Task<string> SaveAnyCriteriaDrivenBudgets(int id, List<CriteriaDrivenBudgetModel> models, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with {id}");
            return SaveCriteriaDrivenBudgets(id, models, db);
        }
    }
}
