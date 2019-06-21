using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Activities.Statements;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using BridgeCare.EntityClasses;

namespace BridgeCare.DataAccessLayer
{
    public class TargetsDAL : ITarget
    {
        private readonly BridgeCareContext db;
        private readonly TargetsMetDAL targets;
        private readonly TargetResultsDAL targetCells;

        public TargetsDAL(BridgeCareContext context, TargetsMetDAL targets, TargetResultsDAL result)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            this.targets = targets ?? throw new ArgumentNullException(nameof(targets));
            targetCells = result ?? throw new ArgumentNullException(nameof(result));
        }

        public TargetReportModel GetTarget(SimulationModel data, int[] totalYears)
        {
            TargetReportModel targetReport = null;

            var select =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientReportModel>(select).AsQueryable();

                var results = rawDeficientList.Where(_ => _.IsDeficient == false);

                var targetAndYear = targets.GetData(results);
                targetReport = GetTargetInformation(data, targetAndYear, totalYears);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Target");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }
            return targetReport;
        }

        private TargetReportModel GetTargetInformation(SimulationModel data, Hashtable yearsIdValues, int[] totalYears)
        {
            var targetData = db.Targets.AsNoTracking().Where(_ => _.SIMULATIONID == data.SimulationId);

            var listRows = new List<string>();
            var idTargets = new Hashtable();

            foreach (var item in targetData)
            {
                var target = new TargetParameters
                {
                    Id = item.ID_,
                    Attribute = item.ATTRIBUTE_,
                    TargetMean = item.TARGETMEAN ?? 0,
                    Name = item.TARGETNAME,
                    Criteria = item.CRITERIA
                };
                var rowKey = target.Attribute + "|" + target.Name + "|" + target.Criteria;

                if (!listRows.Contains(rowKey))
                {
                    listRows.Add(rowKey);
                    target.Row = listRows.Count - 1;
                }
                else
                {
                    target.Row = listRows.IndexOf(rowKey);
                }
                idTargets.Add(target.Id, target);
            }

            var dataForTarget = targetCells.GetData(yearsIdValues, totalYears, idTargets);
            return dataForTarget;
        }

        public List<TargetModel> GetTargets(int simulationId, BridgeCareContext db)
        {
            try
            {
                // check if there are existing targets with the given simulation id
                if (db.Targets.Any(target => target.SIMULATIONID == simulationId))
                {
                    // query for existing targets
                    var targets = db.Targets.Where(target => target.SIMULATIONID == simulationId);
                    // check if there are query results
                    if (targets.Any())
                    {
                        // create TargetModels from existing targets and return
                        var targetModels = new List<TargetModel>();
                        targets.ToList().ForEach(target => targetModels.Add(new TargetModel(target)));

                        return targetModels;
                    }
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "TARGETS");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return new List<TargetModel>();
        }

        public List<TargetModel> SaveTargets(int simulationId, List<TargetModel> data, BridgeCareContext db)
        {
            try
            {
                // query for targets using the simulationId
                var existingTargets = db.Targets.Where(target => target.SIMULATIONID == simulationId).ToList();
                if (existingTargets.Any())
                {
                    existingTargets.ForEach(existingTarget =>
                    {
                        // check for matching target model
                        var targetModel = data.SingleOrDefault(model => model.Id == existingTarget.ID_);
                        if (targetModel != null)
                        {
                            // set target model as matched
                            targetModel.matched = true;
                            // update existing target
                            targetModel.UpdateTarget(existingTarget);
                        }
                        else
                        {
                            // delete existing target
                            db.Entry(existingTarget).State = EntityState.Deleted;
                        }
                    });
                }

                db.SaveChanges();

                // check for any targets that weren't matched
                if (data.Any(targetModel => !targetModel.matched))
                {
                    // get all unmatched target models
                    data.Where(targetModel => !targetModel.matched).ToList().ForEach(targetModel =>
                    {
                        // add new target to db context
                        db.Targets.Add(new TargetsEntity(targetModel));
                    });

                    db.SaveChanges();
                }

                // return all upserted data
                var targets = db.Targets.Where(target => target.SIMULATIONID == simulationId).ToList();
                if (targets.Any())
                {
                    var targetModels = new List<TargetModel>();
                    targets.ForEach(target => targetModels.Add(new TargetModel(target)));

                    return targetModels;
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "TARGETS");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }

            return new List<TargetModel>();
        }
    }
}