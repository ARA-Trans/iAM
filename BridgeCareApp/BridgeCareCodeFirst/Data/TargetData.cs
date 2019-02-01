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
    public class TargetData : ITarget
    {
        private readonly BridgeCareContext db;
        private readonly DeficientOrTarget getTargets;
        private readonly TargetResults getTargetCell;

        public TargetData(BridgeCareContext context, DeficientOrTarget targets, TargetResults targetCells)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            getTargets = targets ?? throw new ArgumentNullException(nameof(targets));
            getTargetCell = targetCells ?? throw new ArgumentNullException(nameof(targetCells));
        }
        public Target GetTarget(SimulationResult data, int[] totalYears)
        {
            IQueryable<DeficientResult> targets = null;
            Target results = null;

            var select =
                "SELECT TargetID, Years, TargetMet, IsDeficient " +
                    " FROM Target_" + data.NetworkId
                    + "_" + data.SimulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientResult>(select).AsQueryable();

                targets = rawDeficientList.Where(_ => _.IsDeficient == false);

                var targetAndYear = getTargets.DeficientTargetList(targets);
                results = GetTargetInformation(data, targetAndYear, totalYears);
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
            return results;
        }
        public Target GetTargetInformation(SimulationResult data, Hashtable YearsIDValues, int[] totalYears)
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

            var dataForTarget = getTargetCell.GetCells(YearsIDValues, totalYears, idTargets);
            return dataForTarget;
        }
    }
}