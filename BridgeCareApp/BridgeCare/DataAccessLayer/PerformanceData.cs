using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class PerformanceData : IPerformance
    {
        public PerformanceData()
        {
        }

        public IQueryable<PerformanceModel> GetPerformance(SimulationModel data, BridgeCareContext db)
        {
            try
            {
                return (db.PERFORMANCE
                    .Where(d => d.SIMULATIONID == data.SimulationId)
                    .Select(p => new PerformanceModel()
                    {
                        PerformanceId = p.PERFORMANCEID,
                        SimulationId = p.SIMULATIONID,
                        Attribute = p.ATTRIBUTE_,
                        EquationName = p.EQUATIONNAME,
                        Criteria = p.CRITERIA,
                        Equation = p.EQUATION,
                        Shift = p.SHIFT,
                        BinaryEquation = p.BINARY_EQUATION,
                        BinaryCriteria = p.BINARY_CRITERIA,
                        Piecwise = p.PIECEWISE,
                        IsFunction = p.ISFUNCTION
                    }).ToList()).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Performance");
            }
            return Enumerable.Empty<PerformanceModel>().AsQueryable();
        }
    }
}