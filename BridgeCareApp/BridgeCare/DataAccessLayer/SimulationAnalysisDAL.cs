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
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");

            var simulation = db.Simulations.Single(s => s.SIMULATIONID == id);

            return new SimulationAnalysisModel(simulation);
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
    }
}