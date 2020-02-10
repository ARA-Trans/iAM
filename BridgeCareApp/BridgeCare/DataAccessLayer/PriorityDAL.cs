using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class PriorityDAL : IPriority
    {
        /// <summary>
        /// Fetches a simulation's priority library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>PriorityLibraryModel</returns>
        private PriorityLibraryModel GetSimulationPriorityLibrary(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations.Include(s => s.PRIORITIES)
                .Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                .Single(s => s.SIMULATIONID == id);

            return new PriorityLibraryModel(simulation);
        }

        /// <summary>
        /// Fetches a simulation's priority library data if it belongs to the user
        /// Throws a RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <returns>PriorityLibraryModel</returns>
        public PriorityLibraryModel GetOwnedSimulationPriorityLibrary(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.UserCanRead(username) && s.SIMULATIONID == id))
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's priorities.");
            return GetSimulationPriorityLibrary(id, db);
        }

        /// <summary>
        /// Fetches a simulation's priority library data regardless of ownership
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <returns>PriorityLibraryModel</returns>
        public PriorityLibraryModel GetAnySimulationPriorityLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");
            return GetSimulationPriorityLibrary(id, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's priority library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">PriorityLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>PriorityLibraryModel</returns>
        private PriorityLibraryModel SaveSimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            var simulation = db.Simulations.Include(s => s.PRIORITIES)
                .Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                .Single(s => s.SIMULATIONID == id);

            if (simulation.PRIORITIES.Any())
            {
                simulation.PRIORITIES.ToList().ForEach(priorityEntity =>
                {
                    var priorityModel = model.Priorities
                        .SingleOrDefault(m => m.Id == priorityEntity.PRIORITYID.ToString());

                    if (priorityModel == null)
                        PriorityEntity.DeleteEntry(priorityEntity, db);
                    else
                    {
                        priorityModel.matched = true;
                        priorityModel.UpdatePriority(priorityEntity);

                        if (priorityEntity.PRIORITYFUNDS.Any())
                        {
                            priorityEntity.PRIORITYFUNDS.ToList().ForEach(priorityFundEntity =>
                            {
                                var priorityFundModel = priorityModel.PriorityFunds
                                    .SingleOrDefault(m => m.Id == priorityFundEntity.PRIORITYFUNDID.ToString());

                                if (priorityFundModel == null)
                                    PriorityFundEntity.DeleteEntry(priorityFundEntity, db);
                                else
                                {
                                    priorityFundModel.matched = true;
                                    priorityFundModel.UpdatePriorityFund(priorityFundEntity);
                                }
                            });
                        }

                        if (priorityModel.PriorityFunds.Any(m => !m.matched))
                            db.PriorityFunds.AddRange(priorityModel.PriorityFunds
                                .Where(priorityFundModel => !priorityFundModel.matched)
                                .Select(priorityFundModel => new PriorityFundEntity(priorityEntity.PRIORITYID, priorityFundModel))
                                .ToList()
                            );
                    }
                });
            }

            if (model.Priorities.Any(m => !m.matched))
                db.Priorities.AddRange(model.Priorities
                    .Where(priorityModel => !priorityModel.matched)
                    .Select(priorityModel => new PriorityEntity(id, priorityModel))
                    .ToList()
                );

            db.SaveChanges();

            return new PriorityLibraryModel(simulation);
        }

        public PriorityLibraryModel SaveOwnedSimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db, string username)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.UserCanModify(username) && s.SIMULATIONID == id))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's priorities.");
            return SaveSimulationPriorityLibrary(model, db);
        }

        public PriorityLibraryModel SaveAnySimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");
            return SaveSimulationPriorityLibrary(model, db);
        }
    }
}
