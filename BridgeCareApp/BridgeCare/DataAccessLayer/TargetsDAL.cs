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
        /// Fetches a specific target from a dynamic table 'Target_{NETWORKID}_{SIMULATIONID}'
        /// and transforms the data into a TargetReportModel
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <param name="totalYears">int[]</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TargetReportModel</returns>
        public TargetReportModel GetTarget(SimulationModel model, int[] totalYears, BridgeCareContext db)
        {
            var select = $"SELECT TargetID, Years, coalesce(TARGETMET, 0) as TargetMet, IsDeficient FROM Target_{model.NetworkId}_{model.SimulationId}";

            var deficientReportResults = db.Database.SqlQuery<DeficientReportModel>(select)
                .AsQueryable().Where(_ => _.IsDeficient == false);

            var targetAndYear = targetsMetDAL.GetData(deficientReportResults);

            return GetTargetInformation(model, targetAndYear, totalYears, db);
        }

        /// <summary>
        /// Transforms dynamic table 'Target_{NETWORKID}_{SIMULATIONID}' data into a TargetReportModel
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
        /// Fetches a simulation's target library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TargetLibraryModel</returns>
        private TargetLibraryModel GetSimulationTargetLibrary(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations.Include(s => s.TARGETS).Single(s => s.SIMULATIONID == id);
            return new TargetLibraryModel(simulation);
        }

        /// <summary>
        /// Fetches a simulation's target library data if it is owned by the user
        /// Throws a RowNotInTableException if no simulation is found for the user
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>TargetLibraryModel</returns>
        public TargetLibraryModel GetOwnedSimulationTargetLibrary(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.UserCanRead(username)))
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's targets.");
            return GetSimulationTargetLibrary(id, db);
        }

        /// <summary>
        /// Fetches a simulation's target library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public TargetLibraryModel GetAnySimulationTargetLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}.");
            return GetSimulationTargetLibrary(id, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's target library data
        /// </summary>
        /// <param name="model">TargetLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TargetLibraryModel</returns>
        private TargetLibraryModel SaveSimulationTargetLibrary(TargetLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

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

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's target library data if the user owns it
        /// Throws a RowNotInTableException if no simulation is found for the user
        /// </summary>
        /// <param name="model">TargetLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="username">Username</param>
        /// <returns>TargetLibraryModel</returns>
        public TargetLibraryModel SaveOwnedSimulationTargetLibrary(TargetLibraryModel model, BridgeCareContext db, string username)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.UserCanModify(username)))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's targets.");
            return SaveSimulationTargetLibrary(model, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's target library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public TargetLibraryModel SaveAnySimulationTargetLibrary(TargetLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}.");
            return SaveSimulationTargetLibrary(model, db);
        }
    }
}
