using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class PerformanceLibraryDAL : IPerformanceLibrary
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PerformanceLibraryDAL));
        /// <summary>
        /// Fetches a simulation's performance library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>PerformanceLibraryModel</returns>
        private PerformanceLibraryModel GetSimulationPerformanceLibrary(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations.Include(s => s.PERFORMANCES).Single(s => s.SIMULATIONID == id);
            return new PerformanceLibraryModel(simulation);
        }

        /// <summary>
        /// Fetches a simulation's performance library data if it belongs to the user
        /// Throws a RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <returns>PerformanceLibraryModel</returns>
        public PerformanceLibraryModel GetOwnedSimulationPerformanceLibrary(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.UserCanRead(username)))
            {
                log.Warn($"User {username} is not authorized to view scenario {id}.");
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's performance.");
            }
            return GetSimulationPerformanceLibrary(id, db);
        }

        /// <summary>
        /// Fetches a simulation's performance library data regardless of ownership
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <returns>PerformanceLibraryModel</returns>
        public PerformanceLibraryModel GetAnySimulationPerformanceLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
            {
                log.Error($"No scenario was found with id {id}");
                throw new RowNotInTableException($"No scenario was found with id {id}");
            }
            return GetSimulationPerformanceLibrary(id, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's performance library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">PerformanceLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>PerformanceLibraryModel</returns>
        private PerformanceLibraryModel SaveSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
            var simulation = db.Simulations.Include(s => s.PERFORMANCES).Single(s => s.SIMULATIONID == id);

            if (simulation.PERFORMANCES.Any())
                simulation.PERFORMANCES.ToList().ForEach(performanceEntity =>
                {
                    var performanceModel = model.Equations
                        .SingleOrDefault(m => m.Id == performanceEntity.PERFORMANCEID.ToString());

                    if (performanceModel == null)
                        PerformanceEntity.DeleteEntry(performanceEntity, db);
                    else
                    {
                        performanceModel.matched = true;
                        performanceModel.UpdatePerformance(performanceEntity);
                    }
                });

            if (model.Equations.Any(m => !m.matched))
                db.Performances.AddRange(model.Equations
                    .Where(performanceModel => !performanceModel.matched)
                    .Select(performanceModel => new PerformanceEntity(id, performanceModel))
                    .ToList()
                );

            db.SaveChanges();

            return new PerformanceLibraryModel(simulation);
        }

        public PerformanceLibraryModel SaveOwnedSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db, string username)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.UserCanModify(username)))
            {
                log.Warn($"User {username} is not authorized to modify scenario {id}.");
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's performance.");
            }
            return SaveSimulationPerformanceLibrary(model, db);
        }

        public PerformanceLibraryModel SaveAnySimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
            {
                log.Error($"No scenario was found with id {id}");
                throw new RowNotInTableException($"No scenario was found with id {id}");
            }
            return SaveSimulationPerformanceLibrary(model, db);
        }
    }
}
