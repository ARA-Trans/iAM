using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class SimulationAnalysisDAL : ISimulationAnalysis
    {
        /// <summary>
        /// Gets a simulation's analysis data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>SimulationAnalysisModel</returns>
        public SimulationAnalysisModel GetSimulationAnalysis(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations.Single(s => s.SIMULATIONID == id);
            return new SimulationAnalysisModel(simulation);
        }

        /// <summary>
        /// Gets a simulation's analysis data, regardless of ownership
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>SimulationAnalysisModel</returns>
        public SimulationAnalysisModel GetAnySimulationAnalysis(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            return GetSimulationAnalysis(id, db);
        }

        /// <summary>
        /// Gets a simulation's analysis data if it is available to the user
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>SimulationAnalysisModel</returns>
        public SimulationAnalysisModel GetOwnedSimulationAnalysis(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id && (s.USERNAME == username || s.USERNAME == null)))
                throw new RowNotInTableException($"User {username} cannot access a simulation with id {id}.");
            return GetSimulationAnalysis(id, db);
        }

        /// <summary>
        /// Updates a simulation's analysis data
        /// </summary>
        /// <param name="model">SimulationAnalysisModel</param>
        /// <param name="db">BridgeCareContext</param>
        public void UpdateSimulationAnalysis(SimulationAnalysisModel model, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == model.Id))
                throw new RowNotInTableException($"No scenario found with id {model.Id}");

            var simulation = db.Simulations.Single(s => s.SIMULATIONID == model.Id);

            model.UpdateSimulationAnalysis(simulation);

            db.SaveChanges();
        }

        public void PartialUpdateOwnedSimulationAnalysis(SimulationAnalysisModel model, BridgeCareContext db, string username, bool updateWeighting = true)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == model.Id && (s.USERNAME == username)))
                throw new RowNotInTableException($"User {username} cannot update analysis of scenario with id {model.Id}.");

            var simulation = db.Simulations.Single(s => s.SIMULATIONID == model.Id);

            model.PartialUpdateSimulationAnalysis(simulation, updateWeighting);

            db.SaveChanges();
        }
    }
}