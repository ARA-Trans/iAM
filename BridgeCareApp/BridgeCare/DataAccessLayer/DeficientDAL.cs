using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
    public class DeficientDAL : IDeficient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DeficientDAL));
        /// <summary>
        /// Fetches a simulation's deficient library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>DeficientLibraryModel</returns>
        private DeficientLibraryModel GetSimulationDeficientLibrary(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations.Include(s => s.DEFICIENTS).Single(s => s.SIMULATIONID == id);

            return new DeficientLibraryModel(simulation);
        }

        /// <summary>
        /// Fetches a simulation's deficient library data if it is owned by the user
        /// Throws a RowNotInTableException if no simulation is found for the user
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>DeficientLibraryModel</returns>
        public DeficientLibraryModel GetOwnedSimulationDeficientLibrary(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.UserCanRead(username)))
            {
                log.Warn($"User {username} is not authorized to view scenario {id}.");
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's deficients.");
            }
            return GetSimulationDeficientLibrary(id, db);
        }

        /// <summary>
        /// Fetches a simulation's deficient library data regardless of ownership
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>DeficientLibraryModel</returns>
        public DeficientLibraryModel GetAnySimulationDeficientLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
            {
                log.Error($"No scenario was found with id {id}.");
                throw new RowNotInTableException($"No scenario was found with id {id}.");
            }
            return GetSimulationDeficientLibrary(id, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's deficient library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">DeficientLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>DeficientLibraryModel</returns>
        private DeficientLibraryModel SaveSimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
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

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's deficient library data if the user owns it
        /// Throws a RowNotInTableException if no simulation is found for the user
        /// </summary>
        /// <param name="model">DeficientLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>DeficientLibraryModel</returns>
        public DeficientLibraryModel SaveOwnedSimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db, string username)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.UserCanModify(username)))
            {
                log.Warn($"User {username} is not authorized to modify scenario {id}.");
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's deficients.");
            }
            return SaveSimulationDeficientLibrary(model, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's deficient library data regardless of ownership
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">DeficientLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>DeficientLibraryModel</returns>
        public DeficientLibraryModel SaveAnySimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
            {
                log.Error($"No scenario was found with id {id}.");
                throw new RowNotInTableException($"No scenario was found with id {id}.");
            }
            return SaveSimulationDeficientLibrary(model, db);
        }
    }
}
