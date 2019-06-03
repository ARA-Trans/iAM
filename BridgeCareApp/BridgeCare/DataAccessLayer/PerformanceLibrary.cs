using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Collections.Generic;
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
        var simulation = db.SIMULATIONS
          .Include(d => d.PERFORMANCES)
          .Single(s => s.SIMULATIONID == selectedScenarioId);

        var performanceLibraryModel = new PerformanceLibraryModel()
        {
          Id = selectedScenarioId,
          Name = simulation.SIMULATION1,
          Description = simulation.COMMENTS,
          Equations = GetEquations(simulation.PERFORMANCES)
        };

        return performanceLibraryModel;
      }
      catch (SqlException ex)
      {
        HandleException.SqlError(ex, "Performance Scenario Library select failed.");
      }
      return new PerformanceLibraryModel();
    }

    private void DeletePerformances(List<PERFORMANCE> deletePerformances, ICollection<PERFORMANCE> performances, BridgeCareContext db)
    {
      foreach (var deletePerformance in deletePerformances)
      {
        db.Entry(deletePerformance).State = EntityState.Deleted;
      }
      db.SaveChanges();
    }

    private static void AddNewPerformances(PerformanceLibraryModel data, ICollection<PERFORMANCE> performances, BridgeCareContext db)
    {
      var performanceIds = performances.Select(p => p.PERFORMANCEID);
      var newEquations = data.Equations.Where(e => !performanceIds.Contains(e.Id));
      foreach (var newEquation in newEquations)
      {
        performances.Add(new PERFORMANCE
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
      db.SaveChanges();
    }

    private void UpsertPerformancesData(PerformanceLibraryModel data, SIMULATION simulation, BridgeCareContext db)
    {
      var equationModels = data.Equations;
      var performancesToDelete = new List<PERFORMANCE>();
      var performances = simulation.PERFORMANCES;
      // update/delete
      foreach (var performance in performances)
      {
        if (equationModels.Any(e => e.Id == performance.PERFORMANCEID))
        {
          var equationModel = equationModels.Find(e => e.Id == performance.PERFORMANCEID);
          performance.ATTRIBUTE_ = equationModel.Attribute;
          performance.EQUATIONNAME = equationModel.EquationName;
          performance.CRITERIA = equationModel.Criteria;
          performance.EQUATION = equationModel.Equation;
          performance.SHIFT = equationModel.Shift;
          performance.PIECEWISE = equationModel.Piecewise;
          performance.ISFUNCTION = equationModel.IsFunction;
        }
        else
        {
          performancesToDelete.Add(performance);
        }
      }

      db.SaveChanges();

      DeletePerformances(performancesToDelete, performances, db);

      AddNewPerformances(data, performances, db);
    }

    private static List<PerformanceLibraryEquationModel> GetEquations(IEnumerable<PERFORMANCE> performances)
    {
      return performances
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
        }).ToList();
    }

    public PerformanceLibraryModel SaveScenarioPerformanceLibrary(PerformanceLibraryModel data, BridgeCareContext db)
    {
      try
      {
        var simulation = db.SIMULATIONS
          .Include(d => d.PERFORMANCES)
          .Single(s => s.SIMULATIONID == data.Id);

        simulation.COMMENTS = data.Description;
        simulation.SIMULATION1 = data.Name;
        db.SaveChanges();

        UpsertPerformancesData(data, simulation, db);

        // Update model to reflect latest ids (inserts)
        data.Equations = GetEquations(simulation.PERFORMANCES);

        return data;
      }
      catch (SqlException ex)
      {
        HandleException.SqlError(ex, "Performance Scenario Library update failed.");
      }
      // Returning empty model in case of any exception.
      return new PerformanceLibraryModel();
    }
  }
}