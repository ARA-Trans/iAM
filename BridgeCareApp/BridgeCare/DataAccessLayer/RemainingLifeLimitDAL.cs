using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
    public class RemainingLifeLimitDAL : IRemainingLifeLimit
    {
        public RemainingLifeLimitLibraryModel GetScenarioRemainingLifeLimitLibrary(int simulationId,
            BridgeCareContext db)
        {
            try
            {
                if (db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                {
                    // query for an existing simulation and include remaining life limits
                    var simulation = db.Simulations.Include(s => s.REMAINING_LIFE_LIMITS)
                        .Single(s => s.SIMULATIONID == simulationId);
                    // return the simulation's data and any remaining life limits data as a RemainingLifeLimitLibraryModel
                    return new RemainingLifeLimitLibraryModel(simulation);
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "REMAINING_LIFE_LIMITS");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return new RemainingLifeLimitLibraryModel();
        }

        public RemainingLifeLimitLibraryModel SaveScenarioRemainingLifeLimitLibrary(RemainingLifeLimitLibraryModel data,
            BridgeCareContext db)
        {
            try
            {
                var simulationId = int.Parse(data.Id);
                
                if (db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                {
                    // query for an existing simulation
                    var simulation = db.Simulations.Include(s => s.REMAINING_LIFE_LIMITS)
                        .Single(s => s.SIMULATIONID == simulationId);
                    // update the simulation comments
                    simulation.COMMENTS = data.Description;
                    // check for existing remaining life limits
                    if (simulation.REMAINING_LIFE_LIMITS.Any())
                    {
                        simulation.REMAINING_LIFE_LIMITS.ToList().ForEach(remainingLifeLimit =>
                        {
                            // check for a RemainingLifeLimitModel that has a matching id with a remaining life limit id
                            var model = data.RemainingLifeLimits.SingleOrDefault(m =>
                                m.Id == remainingLifeLimit.REMAINING_LIFE_ID.ToString());
                            if (model != null)
                            {
                                // update the remaining life limit record with the matched model data
                                model.matched = true;
                                model.Update(remainingLifeLimit);
                            }
                            else
                            {
                                RemainingLifeLimitsEntity.DeleteEntry(remainingLifeLimit, db);
                            }
                        });
                    }
                    // check for models that didn't have a remaining life limit record match
                    if (data.RemainingLifeLimits.Any(model => !model.matched))
                    {
                        // create new remaining life limits from the unmatched models' data
                        db.RemainingLifeLimits
                            .AddRange(data.RemainingLifeLimits
                                .Where(model => !model.matched)
                                .Select(model => new RemainingLifeLimitsEntity(simulationId, model))
                                .ToList()
                            );
                    }
                    // save all changes
                    db.SaveChanges();
                    // return the updated/inserted records as a RemainingLifeLimitLibraryModel
                    return new RemainingLifeLimitLibraryModel(simulation);
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "REMAINING_LIFE_LIMITS");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return new RemainingLifeLimitLibraryModel();
        }
    }
}