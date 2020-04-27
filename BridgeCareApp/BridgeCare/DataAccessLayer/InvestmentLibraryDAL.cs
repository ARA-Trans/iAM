using BridgeCare.EntityClasses;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class InvestmentLibraryDAL : IInvestmentLibrary
    {
        /// <summary>
        /// Fetches a simulation's investment library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>InvestmentLibraryModel</returns>
        private InvestmentLibraryModel GetSimulationInvestmentLibrary(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations
                .Include(s => s.INVESTMENTS)
                .Include(s => s.YEARLYINVESTMENTS)
                .Include(s => s.CriteriaDrivenBudgets)
                .Single(s => s.SIMULATIONID == id);
            return new InvestmentLibraryModel(simulation);
        }

        /// <summary>
        /// Fetches a simulation's investment library data if it is available to the given user
        /// Throws a RowNotInTableException if no simulation is found for that user
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>InvestmentLibraryModel</returns>
        public InvestmentLibraryModel GetPermittedSimulationInvestmentLibrary(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == id).UserCanRead(username))
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's investments.");
            return GetSimulationInvestmentLibrary(id, db);
        }

        /// <summary>
        /// Fetches a simulation's investment library data regardless of ownership
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>InvestmentLibraryModel</returns>
        public InvestmentLibraryModel GetAnySimulationInvestmentLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            return GetSimulationInvestmentLibrary(id, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's investment library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">InvestmentLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>InvestmentLibraryModel</returns>
        private InvestmentLibraryModel SaveSimulationInvestmentLibrary(InvestmentLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            var simulation = db.Simulations
                .Include(s => s.INVESTMENTS)
                .Include(s => s.YEARLYINVESTMENTS)
                .Include(s => s.CriteriaDrivenBudgets)
                .Include(s => s.PRIORITIES).Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                .Single(s => s.SIMULATIONID == id);

            if (simulation.CriteriaDrivenBudgets.Any())
            {
                simulation.CriteriaDrivenBudgets.ToList().ForEach(budget =>
                {
                    db.Entry(budget).State = System.Data.Entity.EntityState.Deleted;
                });
            }

            model.BudgetCriteria.ForEach(budgetModel =>
            {
                simulation.CriteriaDrivenBudgets.Add(new CriteriaDrivenBudgetsEntity(id, budgetModel));
            });

            if (simulation.INVESTMENTS != null)
                model.UpdateInvestment(simulation.INVESTMENTS);

            if (simulation.YEARLYINVESTMENTS.Any())
            {
                simulation.YEARLYINVESTMENTS.ToList().ForEach(yearlyInvestment =>
                {
                    var yearlyInvestmentModel =
                        model.BudgetYears.SingleOrDefault(m => m.Id == yearlyInvestment.YEARID.ToString());

                    if (yearlyInvestmentModel == null)
                        YearlyInvestmentEntity.DeleteEntry(yearlyInvestment, db);
                    else
                    {
                        yearlyInvestmentModel.matched = true;
                        yearlyInvestmentModel.UpdateYearlyInvestment(yearlyInvestment);
                    }
                });
                
            }

            simulation.PRIORITIES.ToList().ForEach(priorityEntity =>
            {
                var budgetsForNewFunds = new List<string>();

                if (priorityEntity.PRIORITYFUNDS.Any())
                    model.BudgetOrder.ForEach(budget =>
                    {
                      budgetsForNewFunds.Add(budget);
                    });

                if (budgetsForNewFunds.Any())
                {
                    priorityEntity.PRIORITYFUNDS
                        .ToList()
                        .ForEach(priorityFundEntity =>
                        {
                            PriorityFundEntity.DeleteEntry(priorityFundEntity, db);
                        });

                    db.PriorityFunds.AddRange(budgetsForNewFunds
                        .Select(budget => new PriorityFundEntity(priorityEntity.PRIORITYID, budget))
                    );
                }
            });

            if (simulation.INVESTMENTS == null)
                db.Investments.Add(new InvestmentsEntity(model));

            if (model.BudgetYears.Any(m => !m.matched))
                db.YearlyInvestments.AddRange(model.BudgetYears
                    .Where(yearlyInvestmentModel => !yearlyInvestmentModel.matched)
                    .Select(yearlyInvestmentModel => new YearlyInvestmentEntity(id, yearlyInvestmentModel))
                    .ToList()
                );

            db.SaveChanges();

            return new InvestmentLibraryModel(simulation);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's investment library data if it is owned by the provided user
        /// Throws a RowNotInTableException if no simulation is found for that user
        /// </summary>
        /// <param name="model">InvestmentLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>InvestmentLibraryModel</returns>
        public InvestmentLibraryModel SavePermittedSimulationInvestmentLibrary(InvestmentLibraryModel model, BridgeCareContext db, string username)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == id).UserCanModify(username))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's investments.");
            return SaveSimulationInvestmentLibrary(model, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's investment library data regardless of ownership
        /// Throws a RowNotInTableException if no simulation is found for that user
        /// </summary>
        /// <param name="model">InvestmentLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>InvestmentLibraryModel</returns>
        public InvestmentLibraryModel SaveAnySimulationInvestmentLibrary(InvestmentLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            return SaveSimulationInvestmentLibrary(model, db);
        }
    }
}
