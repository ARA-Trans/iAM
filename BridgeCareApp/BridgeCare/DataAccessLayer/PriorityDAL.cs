using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
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
        public PriorityLibraryModel GetSimulationPriorityLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");

            var simulation = db.Simulations.Include(s => s.PRIORITIES)
                .Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                .Single(s => s.SIMULATIONID == id);

            return new PriorityLibraryModel(simulation);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's priority library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">PriorityLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>PriorityLibraryModel</returns>
        public PriorityLibraryModel SaveSimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");

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
    }
}