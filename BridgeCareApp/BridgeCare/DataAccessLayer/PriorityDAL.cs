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
                if (db.Simulations.Any(s => s.SIMULATIONID == simulationId) && db.Investments.Any(investment => investment.SIMULATIONID == simulationId))
                {
                    // query for an existing simulation and include priorities and priority funds
                    var simulation = db.Simulations
                        .Include(s => s.PRIORITIES)
                        .Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                        .Single(s => s.SIMULATIONID == simulationId);
                    // return the simulation's data and any priority & priority funds data
                    return new PriorityLibraryModel(simulation);
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
                // check for an existing simulation
                var simulationId = int.Parse(data.Id);
                if (db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                {
                    // query for the simulation and include priorities and priority funds
                    var simulation = db.Simulations.Include(s => s.PRIORITIES)
                        .Include(s => s.PRIORITIES.Select(p => p.PRIORITYFUNDS))
                        .Single(s => s.SIMULATIONID == simulationId);
                    // update the simulation comments
                    simulation.COMMENTS = data.Description;
                    // check if any priorities were found
                    if (simulation.PRIORITIES.Any())
                    {
                        simulation.PRIORITIES.ToList().ForEach(existingPriority =>
                        {
                            // check for a PriorityModel that has a matching id with a priority id
                            var priorityModel = data.Priorities.SingleOrDefault(model => model.Id == existingPriority.PRIORITYID.ToString());
                            if (priorityModel != null)
                            {
                                // update the priority record with the matched model data
                                priorityModel.matched = true;
                                priorityModel.UpdatePriority(existingPriority);
                                // check for existing priority funds on existing priority
                                if (existingPriority.PRIORITYFUNDS.Any())
                                {
                                    existingPriority.PRIORITYFUNDS.ToList().ForEach(existingPriorityFund =>
                                    {
                                        // check for a PriorityFundModel that has a matching id with a priority fund id
                                        var priorityFundModel = priorityModel.PriorityFunds
                                          .SingleOrDefault(model => model.Id == existingPriorityFund.PRIORITYFUNDID.ToString());
                                        if (priorityFundModel != null)
                                        {
                                            // update the priority fund record with the matched model data
                                            priorityFundModel.matched = true;
                                            priorityFundModel.UpdatePriorityFund(existingPriorityFund);
                                        }
                                        else
                                        {
                                            PriorityFundEntity.DeleteEntry(existingPriorityFund, db);
                                        }
                                    });
                                }

                                // check for PriorityFundModels that didn't have a priority fund record match
                                if (priorityModel.PriorityFunds.Any(model => !model.matched))
                                {
                                    // create a new priority funds with the unmatched models' data
                                    db.PriorityFunds.AddRange(priorityModel.PriorityFunds
                                        .Where(model => !model.matched)
                                        .Select(model => new PriorityFundEntity(existingPriority.PRIORITYID, model))
                                        .ToList()
                                    );
                                }
                            }
                            else
                            {
                                PriorityEntity.DeleteEntry(existingPriority, db);
                            }
                        });
                    }

                    // check for any priority models that weren't matched
                    if (data.Priorities.Any(priorityModel => !priorityModel.matched))
                    {
                        // get all unmatched priority models and create new priority entities with the data and insert
                        db.Priorities.AddRange(data.Priorities
                            .Where(priorityModel => !priorityModel.matched)
                            .Select(priorityModel => new PriorityEntity(simulationId, priorityModel))
                            .ToList()
                        );
                    }

                    db.SaveChanges();

                    return new PriorityLibraryModel(simulation);
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