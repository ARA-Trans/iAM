using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.Data
{
    public class Targets : ITarget
    {
        private readonly BridgeCareContext db;
        private readonly TargetsMet targets;
        private readonly TargetResults targetCells;

        public Targets(BridgeCareContext context, TargetsMet targets, TargetResults result)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            this.targets = targets ?? throw new ArgumentNullException(nameof(targets));
            targetCells = result ?? throw new ArgumentNullException(nameof(result));
        }
        public TargetModel GetTarget(SimulationModel data, int[] totalYears)
        {
            IQueryable<DeficientModel> results = null;
            TargetModel target = null;

            var select =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientModel>(select).AsQueryable();

                results = rawDeficientList.Where(_ => _.IsDeficient == false);

                var targetAndYear = targets.GetData(results);
                target = GetTargetInformation(data, targetAndYear, totalYears);
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
                Console.WriteLine(ex);
            }
            return target;
        }
        public TargetModel GetTargetInformation(SimulationModel data, Hashtable YearsIDValues, int[] totalYears)
        {
            var targetData = db.Target.AsNoTracking().Where(_ => _.SimulationID == data.SimulationId);
            

            var listRows = new List<string>();
            var idTargets = new Hashtable();

            foreach (var item in targetData)
            {
                var target = new TargetParameters
                {
                    Id = item.Id_,
                    Attribute = item.Attribute_,
                    TargetMean = item.TargetMean,
                    Name = item.TargetName,
                    Criteria = item.Criteria
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

            var dataForTarget = targetCells.GetCells(YearsIDValues, totalYears, idTargets);
            return dataForTarget;
        }
    }
}