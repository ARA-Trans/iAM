using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class PerformanceLibrary : IPerformanceLibrary
    {
        public PerformanceLibrary()
        {
        }

        public PerformanceLibraryModel GetScenarioPerformanceLibrary(int selectedScenarioId, BridgeCareContext db)
        {
            try
            {
                var performanceLibraryModel = db.SIMULATIONS
                    .Include(d => d.PERFORMANCES)
                    .Where(d => d.SIMULATIONID == selectedScenarioId)
                    .Select(p => new PerformanceLibraryModel()
                    {
                        Id = selectedScenarioId,
                        Name = p.SIMULATION1,
                        Description = p.COMMENTS,
                        Equations = p.PERFORMANCES
                                    .Select(e => new PerformanceLibraryEquation()
                                    {
                                        PerformanceId = e.PERFORMANCEID,
                                        Attribute = e.ATTRIBUTE_,
                                        EquationName = e.EQUATIONNAME,
                                        Criteria = e.CRITERIA,
                                        Equation = e.EQUATION,
                                        Shift = e.SHIFT,
                                        Piecewise = e.PIECEWISE,
                                        IsFunction = e.ISFUNCTION
                                    }).ToList()
                    }).FirstOrDefault();

                return performanceLibraryModel;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Performance Scenario Library select failed.");
            }
            return new PerformanceLibraryModel();
        }

        public void SaveScenarioPerformanceLibrary(PerformanceLibraryModel data, BridgeCareContext db)
        {
            try
            {
                //var performance = db.PERFORMANCE
                //    .Single(_ => _.PERFORMANCEID == data.PerformanceId);

                //performance.ATTRIBUTE_ = data.Performance.Attribute;
                //performance.EQUATIONNAME = data.Performance.EquationName;
                //performance.CRITERIA = data.Performance.Criteria;
                //performance.EQUATION = data.Performance.Equation;
                //performance.SHIFT = data.Performance.Shift;
                //performance.PIECEWISE = data.Performance.Piecwise;
                //performance.ISFUNCTION = data.Performance.IsFunction;

                db.SaveChanges();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Performance Scenario Library update failed.");
            }
            return;
        }
    }
}