using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
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
                                    .Select(e => new PerformanceLibraryEquationModel()
                                    {
                                        Id = e.PERFORMANCEID,
                                        PerformanceLibraryId = e.SIMULATIONID,
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

        public PerformanceLibraryModel SaveScenarioPerformanceLibrary(PerformanceLibraryModel data, BridgeCareContext db)
        {
            try
            {
                var simulation = db.SIMULATIONS
                   .Include(d => d.PERFORMANCES)
                   .Single(_ => _.SIMULATIONID == data.Id);

                simulation.COMMENTS = data.Description;
                simulation.SIMULATION1 = data.Name;
                
                UpsertPerformancesData(data, simulation, db);

                db.SaveChanges();
                return data;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Performance Scenario Library update failed.");
            }
            // Returning empty model in case of any exception.
            return new PerformanceLibraryModel();
        }

        private void UpsertPerformancesData(PerformanceLibraryModel data, SIMULATION simulation, BridgeCareContext db)
        {
            var equationModels = data.Equations;
            // update/delete
            foreach (var performace in simulation.PERFORMANCES)
            {
                var equationModel = equationModels.Find(e => e.Id == performace.PERFORMANCEID);
                if (equationModel == null)
                {
                    db.Entry(performace).State = EntityState.Deleted;
                }
                else
                {
                    performace.ATTRIBUTE_ = equationModel.Attribute;
                    performace.EQUATIONNAME = equationModel.EquationName;
                    performace.CRITERIA = equationModel.Criteria;
                    performace.EQUATION = equationModel.Equation;
                    performace.SHIFT = equationModel.Shift;
                    performace.PIECEWISE = equationModel.Piecewise;
                    performace.ISFUNCTION = equationModel.IsFunction;
                }
            }

            // insert           
            var performanceIds = simulation.PERFORMANCES.Select(p => p.PERFORMANCEID);
            var newEquations = data.Equations.Where(e => !performanceIds.Contains(e.Id));
            foreach (var newEquation in newEquations)
            {
                simulation.PERFORMANCES.Add(new PERFORMANCE
                {
                    ATTRIBUTE_ = newEquation.Attribute,
                    EQUATIONNAME = newEquation.EquationName,
                    CRITERIA = newEquation.Criteria,
                    EQUATION = newEquation.Equation,
                    SHIFT = newEquation.Shift,
                    PIECEWISE = newEquation.Piecewise,
                    ISFUNCTION = newEquation.IsFunction
                });
            }
        }
    }
}