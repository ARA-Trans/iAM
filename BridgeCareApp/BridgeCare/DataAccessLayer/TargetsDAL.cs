using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.EntityClasses;

namespace BridgeCare.DataAccessLayer
{
    public class TargetsDAL : ITarget
    {
        private readonly TargetsMetDAL targetsMetDAL;
        private readonly TargetResultsDAL targetCells;

        public TargetsDAL(TargetsMetDAL targetsMetDAL, TargetResultsDAL targetCells)
        {
            this.targetsMetDAL = targetsMetDAL ?? throw new ArgumentNullException(nameof(targetsMetDAL));
            this.targetCells = targetCells ?? throw new ArgumentNullException(nameof(targetCells));
        }

        /// <summary>
        /// Fetches a specific target from 'Target_{NETWORKID}_{SIMULATIONID}'
        /// and transforms the data into a TargetReportModel
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="totalYears">int[]</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TargetReportModel</returns>
        public TargetReportModel GetTarget(SimulationModel model, int[] totalYears, BridgeCareContext db)
        {
            TargetReportModel targetReport = null;

            var select = $"SELECT TargetID, Years, TargetMet, IsDeficient FROM Target_{model.NetworkId}_{model.SimulationId}";

            var deficientReportResults = db.Database.SqlQuery<DeficientReportModel>(select)
                .AsQueryable().Where(_ => _.IsDeficient == false);

            var targetAndYear = targetsMetDAL.GetData(deficientReportResults);

            return GetTargetInformation(model, targetAndYear, totalYears, db);
        }

        /// <summary>
        /// Transforms 'Target_{NETWORKID}_{SIMULATIONID}' table data into a TargetReportModel
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="yearsIdValues">Hashtable</param>
        /// <param name="totalYears">int[]</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns></returns>
        private TargetReportModel GetTargetInformation(SimulationModel model, Hashtable yearsIdValues, int[] totalYears, BridgeCareContext db)
        {
            var targetData = db.Targets.AsNoTracking().Where(t => t.SIMULATIONID == model.SimulationId);

            var listRows = new List<string>();
            var idTargets = new Hashtable();

            foreach (var targetEntity in targetData)
            {
                var targetParameters = new TargetParameters(targetEntity);

                var rowKey = $"{targetParameters.Attribute}|{targetParameters.Name}|{targetParameters.Criteria}";

                if (listRows.Contains(rowKey))
                    targetParameters.Row = listRows.IndexOf(rowKey);
                else
                {
                    listRows.Add(rowKey);
                    targetParameters.Row = listRows.Count - 1;
                }

                idTargets.Add(targetParameters.Id, targetParameters);
            }

            return targetCells.GetData(yearsIdValues, totalYears, idTargets);
        }

        /// <summary>
        /// Fetches a simulation record with targets
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TargetLibraryModel</returns>
        public TargetLibraryModel GetSimulationTargetLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}.");

            var simulation = db.Simulations.Include(s => s.TARGETS).Single(s => s.SIMULATIONID == id);
            
            return new TargetLibraryModel(simulation);
        }

        /// <summary>
        /// Executes an upsert/delete operation on the targets table
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">TargetLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TargetLibraryModel</returns>
        public TargetLibraryModel SaveSimulationTargetLibrary(TargetLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}.");

            var simulation = db.Simulations.Include(s => s.TARGETS).Single(s => s.SIMULATIONID == id);

            if (simulation.TARGETS.Any())
            {
                simulation.TARGETS.ToList().ForEach(targetEntity =>
                {
                    var targetModel = model.Targets.SingleOrDefault(m => m.Id == targetEntity.ID_.ToString());

                    if (targetModel == null)
                        TargetsEntity.DeleteEntry(targetEntity, db);
                    else
                    {
                        targetModel.matched = true;
                        targetModel.UpdateTarget(targetEntity);
                    }
                });
            }

            if (model.Targets.Any(m => !m.matched))
                db.Targets
                    .AddRange(model.Targets
                        .Where(targetModel => !targetModel.matched)
                        .Select(targetModel => new TargetsEntity(id, targetModel))
                        .ToList()
                    );

            db.SaveChanges();

            return new TargetLibraryModel(simulation);

        }
    }
}