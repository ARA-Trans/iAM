using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
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

        public TargetLibraryModel GetScenarioTargetLibrary(int simulationId, BridgeCareContext db)
        {
            try
            {
                if (db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                {
                    // query an existing simulation and include targets
                    var simulation = db.Simulations.Include(s => s.TARGETS)
                        .Single(s => s.SIMULATIONID == simulationId);
                    // return the simulation's data and any targets data as a TargetLibraryModel
                    return new TargetLibraryModel(simulation);
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

            return new TargetLibraryModel();
        }

        public TargetLibraryModel SaveScenarioTargetLibrary(TargetLibraryModel data, BridgeCareContext db)
        {
            try
            {
                var simulationId = int.Parse(data.Id);

                if (db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                {
                    // query for an existing simulation
                    var simulation = db.Simulations.Include(s => s.TARGETS)
                        .Single(s => s.SIMULATIONID == simulationId);
                    // update the simulation comments
                    // simulation.COMMENTS = data.Description;
                    if (simulation.TARGETS.Any())
                    {
                        simulation.TARGETS.ToList().ForEach(target =>
                        {
                            // check for a TargetModel that has a matching id with a target id
                            var model = data.Targets.SingleOrDefault(m => m.Id == target.ID_.ToString());
                            if (model != null)
                            {
                                // update the target record ith the matched model data
                                model.matched = true;
                                // update existing target
                                model.UpdateTarget(target);
                            }
                            else
                            {
                                TargetsEntity.DeleteEntry(target, db);
                            }
                        });
                    }
                    // check for models that didn't have a target record match
                    // check for any targets that weren't matched
                    if (data.Targets.Any(model => !model.matched))
                    {
                        // get new targets from the unmatched models' data
                        db.Targets
                            .AddRange(data.Targets
                                .Where(model => !model.matched)
                                .Select(model => new TargetsEntity(simulationId, model))
                                .ToList()
                            );
                    }
                    // save all changes
                    db.SaveChanges();
                    // return the updated/inserted records as TargetLibraryModel
                    return new TargetLibraryModel(simulation);
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

            return new TargetLibraryModel();
        }
    }
}