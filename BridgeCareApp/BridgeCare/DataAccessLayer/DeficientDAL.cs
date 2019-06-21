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
        public List<DeficientModel> GetDeficients(int scenarioId, BridgeCareContext db)
        {
            try
            {
                // check if there are existing deficients with a simulation id that matches the given scenarioId
                if (db.Deficients.Any(deficient => deficient.SIMULATIONID == scenarioId))
                {
                    // query for existing deficients
                    var deficients = db.Deficients.Where(deficient => deficient.SIMULATIONID == scenarioId);
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
        public List<DeficientModel> SaveDeficients(int scenarioId, List<DeficientModel> data, BridgeCareContext db)
        {
            try
            {
                // query for existing deficients by matching the simulation id with the given scenarioId
                var existingDeficients = db.Deficients.Where(deficient => deficient.SIMULATIONID == scenarioId);
                if (existingDeficients.Any())
                {
                    existingDeficients.ToList().ForEach(existingDeficient =>
                    {
                        // check for matching deficient model
                        var deficientModel = data.SingleOrDefault(deficient => deficient.Id == existingDeficient.ID_);
                        if (deficientModel != null)
                        {
                            // set deficient model as matched
                            deficientModel.matched = true;
                            // update existing deficient
                            deficientModel.UpdateDeficient(existingDeficient);
                        }
                        else
                        {
                            // delete existing deficient
                            db.Entry(existingDeficient).State = EntityState.Deleted;
                        }
                    });
                }

                db.SaveChanges();

                // check for any deficients that weren't matched
                if (data.Any(deficientModel => !deficientModel.matched))
                {
                    // get all unmatched deficient models
                    data.Where(deficientModel => !deficientModel.matched).ToList().ForEach(deficientModel =>
                    {
                        // add new deficient to db context
                        db.Deficients.Add(new DeficientsEntity(deficientModel));
                    });

                    db.SaveChanges();
                }

                // return all upserted data
                var deficients = db.Deficients.Where(deficient => deficient.SIMULATIONID == scenarioId).ToList();
                if (deficients.Any())
                {
                    var deficientModels = new List<DeficientModel>();
                    deficients.ForEach(deficient => deficientModels.Add(new DeficientModel(deficient)));

                    return deficientModels;
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
    }
}