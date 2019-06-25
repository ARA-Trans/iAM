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
    public class DeficientDAL : IDeficient
    {
        /// <summary>
        /// Queries for deficients having the specified scenario id foreign key; returns an empty list if no deficients are found
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<DeficientModel> GetDeficients(int simulationId, BridgeCareContext db)
        {
            try
            {
                // check if there are existing deficients with a simulation id that matches the given scenarioId
                if (db.Deficients.Any(deficient => deficient.SIMULATIONID == simulationId))
                {
                    // query for existing deficients
                    var deficients = db.Deficients.Where(deficient => deficient.SIMULATIONID == simulationId);
                    if (deficients.Any())
                    {
                        // create DeficientModels from existing deficients and return
                        var deficientModels = new List<DeficientModel>();
                        deficients.ToList().ForEach(deficient => deficientModels.Add(new DeficientModel(deficient)));
                        return deficientModels;
                    }
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "DEFICIENTS");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return new List<DeficientModel>();
        }

        /// <summary>
        /// Performs an upsert/delete operation on the DEFICIENTS table using the provided list of DeficientModel data
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="data"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<DeficientModel> SaveDeficients(int simulationId, List<DeficientModel> data, BridgeCareContext db)
        {
            try
            {
                // query for existing deficients by matching the simulation id with the given scenarioId
                var existingDeficients = db.Deficients.Where(deficient => deficient.SIMULATIONID == simulationId).ToList();
                if (existingDeficients.Any())
                {
                    existingDeficients.ForEach(existingDeficient =>
                    {
                        // check for matching deficient model
                        var deficientModel = data.SingleOrDefault(deficient => deficient.Id == existingDeficient.ID_.ToString());
                        if (deficientModel != null)
                        {
                            // set deficient model as matched
                            deficientModel.matched = true;
                            // update existing deficient
                            deficientModel.UpdateDeficient(existingDeficient);
                        }
                    });
                }

                // check for any deficient models that weren't matched
                if (data.Any(deficientModel => !deficientModel.matched))
                {
                    // get all unmatched deficient models and create new deficient entities with the data and insert
                    db.Deficients.AddRange(data.Where(deficientModel => !deficientModel.matched).Select(deficientModel => new DeficientsEntity(deficientModel)).ToList());
                }

                db.SaveChanges();

                // if there are any existing deficients, get all of their ids into a list and add the entities to a list as deficient models
                var deficientModels = new List<DeficientModel>();
                var existingDeficientIds = new List<int>();
                if (existingDeficients.Any())
                {
                    deficientModels.AddRange(existingDeficients.Select(deficient => new DeficientModel(deficient)).ToList());
                    existingDeficientIds.AddRange(existingDeficients.Select(deficient => deficient.ID_).ToList());
                }
                // if there are new deficients, create deficient models from them and add them to the deficientModels list
                var newDeficients = db.Deficients
                    .Where(deficient => deficient.SIMULATIONID == simulationId && !existingDeficientIds.Contains(deficient.ID_)).ToList();
                if (newDeficients.Any())
                {
                    // convert all new deficients into deficient models
                    deficientModels.AddRange(newDeficients.Select(deficient => new DeficientModel(deficient)).ToList());
                }

                return deficientModels;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "DEFICIENTS");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return new List<DeficientModel>();
        }
    }
}