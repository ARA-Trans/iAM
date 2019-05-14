using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class SimulationAnalysis : ISimulationAnalysis
    {
        public SimulationAnalysis()
        {
        }

        public SimulationAnalysisModel GetSimulationAnalyis(int simulationId, BridgeCareContext db)
        {
            try
            {
                var simulationAnalysisModel = db.SIMULATIONS
                    .Where(b => b.SIMULATIONID == simulationId)
                    .Select(b => new SimulationAnalysisModel()
                    {
                        simulationId = b.SIMULATIONID,
                        committed_start = b.COMMITTED_START,
                        committed_period = b.COMMITTED_PERIOD,
                        analysis = b.ANALYSIS,
                        budget_constraint = b.BUDGET_CONSTRAINT,
                        benefit_limit = b.BENEFIT_LIMIT,
                        comments = b.COMMENTS,
                        jurisdiction = b.JURISDICTION,
                        benefit_variable = b.BENEFIT_VARIABLE
                    }).SingleOrDefault();

                return simulationAnalysisModel;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Get Simulation Analysis Failed");
            }

            return new SimulationAnalysisModel();
        }

        public void UpdateSimulationAnalyis(SimulationAnalysisModel model, BridgeCareContext db)
        {
            try
            {
                var simulation = db.SIMULATIONS.FirstOrDefault(p => p.SIMULATIONID == model.simulationId);

                simulation.JURISDICTION = model.jurisdiction;
                simulation.ANALYSIS = model.analysis;
                simulation.BENEFIT_LIMIT = model.benefit_limit;
                simulation.BUDGET_CONSTRAINT = model.budget_constraint;
                simulation.COMMENTS = model.comments;
                simulation.COMMITTED_PERIOD = model.committed_period;
                simulation.COMMITTED_START = model.committed_start;
                simulation.BENEFIT_VARIABLE = model.benefit_variable;

                db.SaveChanges();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Simulation anlalysis");
            }
        }
    }
}