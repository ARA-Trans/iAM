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
        /// Queries for the priorities having the specified scenario id foreign key; returns an empty list if no priorities were found
        /// </summary>
        /// <param name="simulationId">int; simulation id</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns></returns>
        public List<PriorityModel> GetPriorities(int simulationId, BridgeCareContext db)
        {
            try
            {
                // check if there are existing priorities with the given simulation id
                if (db.Priorities.Any(priority => priority.SIMULATIONID == simulationId))
                {
                    // query for existing priorities and their priority funds
                    var priorities = db.Priorities
                      .Include("PRIORITYFUNDS")
                      .Where(priority => priority.SIMULATIONID == simulationId);
                    // check if there are query results
                    if (priorities.Any())
                    {
                        // create PriorityModels from existing priorities and return
                        var priorityModels = new List<PriorityModel>();
                        priorities.ToList().ForEach(priority => priorityModels.Add(new PriorityModel(priority)));

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
        /// Performs an upsert/delete operation on the PRIORITY/PRIORITYFUND tables using the provided list of PriorityModel data
        /// </summary>
        /// <param name="data">List<PriorityModel></param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns></returns>
        public List<PriorityModel> SavePriorities(int simulationId, List<PriorityModel> data, BridgeCareContext db)
        {
            try
            {
                // query for priorities using the simulation id
                var existingPriorities = db.Priorities.Include("PRIORITYFUNDS").Where(priority => priority.SIMULATIONID == simulationId).ToList();
                // check if any priorities were found
                if (existingPriorities.Any())
                {
                    existingPriorities.ForEach(existingPriority =>
                    {
                        // check for matching priority model
                        var priorityModel = data.SingleOrDefault(model => model.Id == existingPriority.PRIORITYID.ToString());
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
                if (data.Any(priorityModel => !priorityModel.matched))
                {
                    // get all unmatched priority models and create new priority entities with the data and insert
                    db.Priorities.AddRange(data.Where(priorityModel => !priorityModel.matched).Select(priorityModel => new PriorityEntity(priorityModel)).ToList());
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
                    .Where(priority => priority.SIMULATIONID == simulationId && !existingPriorityIds.Contains(priority.PRIORITYID)).ToList();
                if (newPriorities.Any())
                {
                    // convert all new priorities into priority models
                    priorityModels.AddRange(newPriorities.Select(priority => new PriorityModel(priority)).ToList());
                }
                
                return priorityModels;
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