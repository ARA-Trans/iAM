using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class PerformanceLibraryDAL : IPerformanceLibrary
    {
        /// <summary>
        /// Fetches a simulation's performance library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>PerformanceLibraryModel</returns>
        public PerformanceLibraryModel GetSimulationPerformanceLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");

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
            if (!db.Simulations.Any(s => s.SIMULATIONID == id && (s.USERNAME == username || s.USERNAME == null)))
            {
                throw new RowNotInTableException($"User {username} does not have access to a scenario with id {id}.");
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
        public PerformanceLibraryModel SaveSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");

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
            if (!db.Simulations.Any(s => s.SIMULATIONID == id && s.USERNAME == username))
            {
                throw new RowNotInTableException($"User {username} does not have access to a scenario with id {id}.");
            }
            return SaveSimulationPerformanceLibrary(model, db);
        }
    }
}