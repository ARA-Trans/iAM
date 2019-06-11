using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
  public class Priority : IPriority
  {
    /// <summary>
    /// Queries for the priorities having the specified scenario id foreign key; returns an empty list if no priorities were found
    /// </summary>
    /// <param name="selectedScenarioId">int; scenario (simulation) id</param>
    /// <param name="db">BridgeCareContext</param>
    /// <returns></returns>
    public List<PriorityModel> GetPriorities(int selectedScenarioId, BridgeCareContext db)
    {
      try
      {
        if (db.Priorities.Any(p => p.SIMULATIONID == selectedScenarioId))
        {
          var priorities = db.Priorities
            .Include(pf => pf.PRIORITYFUNDS)
            .Where(p => p.SIMULATIONID == selectedScenarioId);

          if (priorities.Any())
          {
            var priorityModels = new List<PriorityModel>();
            priorities.ToList().ForEach(p => priorityModels.Add(new PriorityModel(p)));
            return priorityModels;
          }
        }
      }
      catch (SqlException ex)
      {
        HandleException.SqlError(ex, "PRIORITY");
      }
      catch (OutOfMemoryException ex)
      {
        HandleException.OutOfMemoryError(ex);
      }
      catch (Exception ex)
      {
        HandleException.GeneralError(ex);
      }
      
      return new List<PriorityModel>();
    }

    /// <summary>
    /// Performs an upsert/delete operation on the PRIORITY/PRIORITYFUND tables using the provided list of priority models data
    /// </summary>
    /// <param name="data">List<PriorityModel></param>
    /// <param name="db">BridgeCareContext</param>
    /// <returns></returns>
    public List<PriorityModel> SavePriorities(List<PriorityModel> data, BridgeCareContext db)
    {
      try
      {
        var existingPriorities = db.Priorities.Where(p => p.SIMULATIONID == data.First().ScenarioId).ToList();
        if (existingPriorities.Any())
        {
          existingPriorities.ForEach(existingPriority =>
          {
            var priorityModel = data.SingleOrDefault(p => p.Id == existingPriority.PRIORITYID);
            if (priorityModel != null)
            {
              // set priorityModel as matched
              priorityModel.matched = true;
              // update existingPriority
              existingPriority.PRIORITYLEVEL = priorityModel.PriorityLevel;
              existingPriority.YEARS = priorityModel.Year;
              existingPriority.CRITERIA = priorityModel.Criteria;

              if (priorityModel.PriorityFunds.Any())
              {
                existingPriority.PRIORITYFUNDS.ToList().ForEach(existingPriorityFund =>
                {
                  var priorityFundModel =
                    priorityModel.PriorityFunds.SingleOrDefault(pf => pf.Id == existingPriorityFund.PRIORITYFUNDID);
                  if (priorityFundModel != null)
                  {
                    // update existingPriorityFund
                    priorityFundModel.matched = true;
                    existingPriorityFund.BUDGET = priorityFundModel.Budget;
                    existingPriorityFund.FUNDING = priorityFundModel.Funding;
                  }
                });
              }
            }
            else
            {
              // mark existingPriority's PRIORITYFUNDS as deleted
              existingPriority.PRIORITYFUNDS.ToList()
                .ForEach(priorityFund => db.Entry(priorityFund).State = EntityState.Deleted);
              // mark existingPriority as deleted
              db.Entry(existingPriority).State = EntityState.Deleted;
            }
          });
          
        }
      }
      catch (SqlException ex)
      {
        HandleException.SqlError(ex, "PRIORITY");
      }
      catch (OutOfMemoryException ex)
      {
        HandleException.OutOfMemoryError(ex);
      }
      catch (Exception ex)
      {
        HandleException.GeneralError(ex);
      }
      return new List<PriorityModel>();
    }
  }
}