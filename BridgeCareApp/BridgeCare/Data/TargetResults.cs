﻿using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.Data
{
    public class TargetResults
    {
        private readonly BridgeCareContext db;
        private readonly CellAddress address;

        public TargetResults(BridgeCareContext context, CellAddress cell)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            address = cell ?? throw new ArgumentNullException(nameof(cell));
        }

        public TargetModel GetData(Hashtable YearsIDValues, int[] totalYears, Hashtable idTargets)
        {
            var attributeData = db.Attributes.AsNoTracking();
            var attributeOrder = new Dictionary<string, bool>();

            foreach (var attribute in attributeData)
            {
                attributeOrder.Add(attribute.Attribute_, attribute.Ascending);
            }

            var totalYearCount = totalYears.Count();
            DataTable targetTable = new DataTable();
            targetTable.Columns.Add("Attribute");
            targetTable.Columns.Add("Group");
            for (int i = 0; i < totalYearCount; i++)
            {
                targetTable.Columns.Add(totalYears[i].ToString());
            }

            int increment = 2;
            foreach (var key in YearsIDValues.Keys)
            {
                Hashtable yearHashValue = new Hashtable();
                yearHashValue = (Hashtable)YearsIDValues[key];
                TargetParameters target = (TargetParameters)idTargets[key];

                DataRow newDataRow = targetTable.NewRow();
                newDataRow["Attribute"] = target.Attribute;
                newDataRow["Group"] = target.Name;

                int column = 2;
                foreach (int year in yearHashValue.Keys)
                {
                    double targetMetValue = (double)yearHashValue[year];
                    newDataRow[column] = targetMetValue + "/" + target.TargetMean;
                    if (attributeOrder.ContainsKey(target.Attribute) && !attributeOrder[target.Attribute])
                    {
                        if (targetMetValue < target.TargetMean)
                        {
                            var cell = (row: increment, column: column);
                            address.Cells.Add(cell);
                        }
                    }
                    else
                    {
                        if (targetMetValue > target.TargetMean)
                        {
                            var cell = (row: increment, column: column);
                            address.Cells.Add(cell);
                        }
                    }
                    column++;
                }
                targetTable.Rows.Add(newDataRow);
                increment++;
            }
            var dataForTarget = new TargetModel
            {
                Targets = targetTable,
                Address = address
            };
            return dataForTarget;
        }
    }
}