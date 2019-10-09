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
        /// Fetches a simulation's deficient library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>DeficientLibraryModel</returns>
        public DeficientLibraryModel GetSimulationDeficientLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}.");

            var simulation = db.Simulations.Include(s => s.DEFICIENTS).Single(s => s.SIMULATIONID == id);

            return new DeficientLibraryModel(simulation);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's deficient library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">DeficientLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>DeficientLibraryModel</returns>
        public DeficientLibraryModel SaveSimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}.");

            var simulation = db.Simulations.Include(s => s.DEFICIENTS).Single(s => s.SIMULATIONID == id);

            if (simulation.DEFICIENTS.Any())
                simulation.DEFICIENTS.ToList().ForEach(deficientEntity =>
                {
                    var deficientModel = model.Deficients.SingleOrDefault(m => m.Id == deficientEntity.ID_.ToString());

                    if (deficientModel == null)
                        DeficientsEntity.DeleteEntry(deficientEntity, db);
                    else
                    {
                        deficientModel.matched = true;
                        deficientModel.UpdateDeficient(deficientEntity);
                    }
                });

            if (model.Deficients.Any(m => !m.matched))
                db.Deficients
                    .AddRange(model.Deficients
                        .Where(deficientModel => !deficientModel.matched)
                        .Select(deficientModel => new DeficientsEntity(id, deficientModel))
                        .ToList()
                    );

            db.SaveChanges();

            return new DeficientLibraryModel(simulation);
        }
    }
}