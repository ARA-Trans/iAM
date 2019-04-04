using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class TreatemntData : ITreatment
    {
        public TreatmentData()
        {
        }

        public IQueryable<TreatmentScenarioModel> GetPerformance(SimulationModel data, BridgeCareContext db)
        {
            try
            {
                return (db.PERFORMANCE
                    .Where(d => d.SIMULATIONID == data.SimulationId)
                    .Select(p => new PerformanceScenarioModel()
                    {
                        PerformanceId = p.PERFORMANCEID,
                        SimulationId = p.SIMULATIONID,
                        Performance = new TreatmentModel()
                        {
                            Attribute = p.ATTRIBUTE_,
                            EquationName = p.EQUATIONNAME,
                            Criteria = p.CRITERIA,
                            Equation = p.EQUATION,
                            Shift = p.SHIFT,
                            Piecwise = p.PIECEWISE,
                            IsFunction = p.ISFUNCTION
                        }
                    }).ToList()).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Treatment");
            }
            return Enumerable.Empty<TreatmentScenarioModel>().AsQueryable();
        }

        public void UpdateTreatmentScenario(TreatmentScenarioModel data, BridgeCareContext db)
        {
            try
            {
                var performance = db.PERFORMANCE
                    .Single(_ => _.PERFORMANCEID == data.PerformanceId);

                performance.ATTRIBUTE_ = data.Performance.Attribute;
                performance.EQUATIONNAME = data.Performance.EquationName;
                performance.CRITERIA = data.Performance.Criteria;
                performance.EQUATION = data.Performance.Equation;
                performance.SHIFT = data.Performance.Shift;
                performance.PIECEWISE = data.Performance.Piecwise;
                performance.ISFUNCTION = data.Performance.IsFunction;

                db.SaveChanges();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Treatment Scenario");
            }
            return;
        }
    }
}