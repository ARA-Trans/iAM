using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
    public class DeficientDAL : IDeficient
    {
        /// <summary>
        /// Queries for deficients having the specified scenario id foreign key; returns an empty list if no deficients are found
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public DeficientLibraryModel GetScenarioDeficientLibrary(int simulationId, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                throw new RowNotInTableException($"No scenario was found with id {simulationId}.");

            // query for an existing simulation and include deficients
            var simulation = db.Simulations.Include(s => s.DEFICIENTS)
                .Single(s => s.SIMULATIONID == simulationId);
            // return the simulation's data and any deficients data as a DeficientLibraryModel
            return new DeficientLibraryModel(simulation);
        }

        /// <summary>
        /// Performs an upsert/delete operation on the DEFICIENTS table using the provided list of DeficientModel data
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="data"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public DeficientLibraryModel SaveScenarioDeficientLibrary(DeficientLibraryModel data, BridgeCareContext db)
        {
            var simulationId = int.Parse(data.Id);

            if (db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                throw new RowNotInTableException($"No scenario was found with id {simulationId}.");
            // query for an existing simulation
            var simulation = db.Simulations.Include(s => s.DEFICIENTS)
                .Single(s => s.SIMULATIONID == simulationId);
            // update the simulation comments
            // simulation.COMMENTS = data.Description;
            if (simulation.DEFICIENTS.Any())
            {
                simulation.DEFICIENTS.ToList().ForEach(deficient =>
                {
                    // check for a DeficientModel that has a matching id with a deficient id
                    var model = data.Deficients.SingleOrDefault(m => m.Id == deficient.ID_.ToString());
                    if (model != null)
                    {
                        // update the deficient record with the matched model data
                        model.matched = true;
                        model.UpdateDeficient(deficient);
                    }
                    else
                    {
                        DeficientsEntity.DeleteEntry(deficient, db);
                    }
                });
            }
            // check for models that didn't have a deficient record match
            if (data.Deficients.Any(model => !model.matched))
            {
                // create new deficients from unmatched models' data
                db.Deficients
                    .AddRange(data.Deficients
                        .Where(model => !model.matched)
                        .Select(model => new DeficientsEntity(simulationId, model))
                        .ToList()
                    );
            }
            // save all changes
            db.SaveChanges();
            // return the updated/inserted records as a DeficientLibraryModel
            return new DeficientLibraryModel(simulation);
        }
    }
}