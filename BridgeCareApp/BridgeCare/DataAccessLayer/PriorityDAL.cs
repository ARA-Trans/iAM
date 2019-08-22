using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class PriorityDAL : IPriority
    {
        /// <summary>
        /// Queries for the priorities having the specified simulation id foreign key; returns an PriorityLibraryModel if no priorities found
        /// </summary>
        /// <param name="simulationId">int; simulation id</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns></returns>
        public PriorityLibraryModel GetScenarioPriorityLibrary(int simulationId, BridgeCareContext db)
        {
            try
            {
                if (db.Investments.Any(investment => investment.SIMULATIONID == simulationId))
                {
                    // query for existing priorities and their priority funds
                    var simulation = db.Simulations
                        .Include(s => s.PRIORITIES)
                        .Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                        .SingleOrDefault(s => s.SIMULATIONID == simulationId);

                    if (simulation != null)
                    {
                        // create PriorityModels from existing priorities and return
                        var priorityModels = new List<PriorityModel>();
                        if (simulation.PRIORITIES.Any())
                        {
                            simulation.PRIORITIES.ToList().ForEach(priority => priorityModels.Add(new PriorityModel(priority)));
                        }

                        return new PriorityLibraryModel(simulation, priorityModels);
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

            return new PriorityLibraryModel();
        }

        /// <summary>
        /// Performs an upsert/delete operation on the PRIORITY/PRIORITYFUND tables using the provided PriorityLibraryModel data
        /// </summary>
        /// <param name="data">List<PriorityLibraryModel></param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns></returns>
        public PriorityLibraryModel SaveScenarioPriorityLibrary(PriorityLibraryModel data, BridgeCareContext db)
        {
            try
            {
                if (db.Simulations.Any(s => s.SIMULATIONID == data.Id))
                {
                    var simulation = db.Simulations.Single(s => s.SIMULATIONID == data.Id);
                    simulation.COMMENTS = data.Description;
                    // query for priorities using the simulation id
                    var existingPriorities = db.Priorities.Include(priority => priority.PRIORITYFUNDS).Where(priority => priority.SIMULATIONID == data.Id).ToList();
                    // check if any priorities were found
                    if (existingPriorities.Any())
                    {
                        existingPriorities.ForEach(existingPriority =>
                        {
                            // check for matching priority model
                            var priorityModel = data.Priorities.SingleOrDefault(model => model.Id == existingPriority.PRIORITYID.ToString());
                            if (priorityModel != null)
                            {
                                // set priority model as matched
                                priorityModel.matched = true;
                                // update existingPriority
                                priorityModel.UpdatePriority(existingPriority);
                                // check for existing priority funds on existing priority
                                if (existingPriority.PRIORITYFUNDS.Any())
                                {
                                    existingPriority.PRIORITYFUNDS.ToList().ForEach(existingPriorityFund =>
                                    {
                                        // check for matching priority fund model
                                        var priorityFundModel = priorityModel.PriorityFunds
                                          .SingleOrDefault(model => model.Id == existingPriorityFund.PRIORITYFUNDID.ToString());
                                        if (priorityFundModel != null)
                                        {
                                            // set priority fund model as matched
                                            priorityFundModel.matched = true;
                                            // update existing priority fund
                                            priorityFundModel.UpdatePriorityFund(existingPriorityFund);
                                        }
                                    });
                                }

                                // check for priority fund models that were not matched
                                if (priorityModel.PriorityFunds.Any(model => !model.matched))
                                {
                                    // create a new priority fund for the existing priority
                                    priorityModel.PriorityFunds.Where(model => !model.matched)
                                        .ToList().ForEach(model =>
                                        {
                                            db.PriorityFunds
                                                .Add(new PriorityFundEntity(existingPriority.PRIORITYID, model));
                                        });
                                }
                            }
                        });

                    }

                    // check for any priority models that weren't matched
                    if (data.Priorities.Any(priorityModel => !priorityModel.matched))
                    {
                        // get all unmatched priority models and create new priority entities with the data and insert
                        db.Priorities.AddRange(data.Priorities.Where(priorityModel => !priorityModel.matched).Select(priorityModel => new PriorityEntity(data.Id, priorityModel)).ToList());
                    }

                    db.SaveChanges();

                    // if there are any existing priorities, get all of their ids into a list and add the entities to a list as priority models
                    var priorityModels = new List<PriorityModel>();
                    var existingPriorityIds = new List<int>();
                    if (existingPriorities.Any())
                    {
                        priorityModels.AddRange(existingPriorities.Select(priority => new PriorityModel(priority)).ToList());
                        existingPriorityIds.AddRange(existingPriorities.Select(priority => priority.PRIORITYID).ToList());
                    }
                    // if there are any new priorities, create priority models from them and add them to the priorityModels list
                    var newPriorities = db.Priorities.Include("PRIORITYFUNDS")
                        .Where(priority => priority.SIMULATIONID == data.Id && !existingPriorityIds.Contains(priority.PRIORITYID)).ToList();
                    if (newPriorities.Any())
                    {
                        // convert all new priorities into priority models
                        priorityModels.AddRange(newPriorities.Select(priority => new PriorityModel(priority)).ToList());
                    }

                    data.Priorities = priorityModels;
                    return data;
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
            
            return new PriorityLibraryModel();
        }

        public void SavePriorityFundInvestmentData(int simulationId, List<string> budgets, BridgeCareContext db)
        {
            if (db.Priorities.Any(priority => priority.SIMULATIONID == simulationId))
            {
                var priorities = db.Priorities.Include(priority => priority.PRIORITYFUNDS)
                    .Where(priority => priority.SIMULATIONID == simulationId).ToList();

                priorities.ForEach(priority =>
                {
                    if (priority.PRIORITYFUNDS.Any())
                    {
                        priority.PRIORITYFUNDS.ToList().ForEach(priorityFund =>
                        {
                            if (!budgets.Contains(priorityFund.BUDGET))
                            {
                                db.Entry(priorityFund).State = EntityState.Deleted;
                            }
                            else
                            {
                                budgets.Remove(priorityFund.BUDGET);
                            }
                        });
                    }

                    budgets.ForEach(budget =>
                    {
                        priority.PRIORITYFUNDS.Add(new PriorityFundEntity()
                        {
                            PRIORITYID = priority.PRIORITYID,
                            BUDGET = budget,
                            FUNDING = 100
                        });
                    });
                });
            }
            else
            {
                List<PriorityFundEntity> newPriorityFunds = new List<PriorityFundEntity>();

                budgets.ForEach(budget =>
                {
                    newPriorityFunds.Add(new PriorityFundEntity()
                    {
                        BUDGET = budget,
                        FUNDING = 100
                    });
                });

                db.Priorities.Add(new PriorityEntity()
                {
                    SIMULATIONID = simulationId,
                    PRIORITYLEVEL = 1,
                    CRITERIA = "",
                    PRIORITYFUNDS = newPriorityFunds,
                    YEARS = DateTime.Now.Year
                });
            }

            db.SaveChanges();
        }

        /*public void Dispose()
        {
            throw new NotImplementedException();
        }*/
    }
}