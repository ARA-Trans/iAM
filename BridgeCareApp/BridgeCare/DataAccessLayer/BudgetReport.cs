using BridgeCare.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.ApplicationLog;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
    public class BudgetReport : IBudgetReport
    {
        private readonly List<CostDetails> costs;
        private readonly BridgeCareContext db;

        public BudgetReport(List<CostDetails> cost, BridgeCareContext context)
        {
            costs = cost ?? throw new ArgumentNullException(nameof(cost));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }
        public YearlyBudgetAndCost GetData(SimulationModel data, string[] budgetTypes)
        {
            var budgetYearView = new Hashtable();
            var select =
                "SELECT Years, Budget, Cost_ " +
                    " FROM Report_" + data.NetworkId
                    + "_" + data.SimulationId + " WHERE BUDGET is not null";
            try
            {
                var rawQueryForData = db.Database.SqlQuery<BudgetModel>(select).AsQueryable();

                double sum = 0;
                foreach (var row in rawQueryForData)
                {
                    double.TryParse(row.Cost_.Value.ToString(), out double cost);
                    Hashtable yearView;
                    //[NOTE].. Filling up the hash table. Data is coming from the database in such a way
                    // that an Hashtable cannot be filled in one go and the checks are necessary.
                    if (!budgetYearView.Contains(row.Budget))
                    {
                        yearView = new Hashtable();
                        budgetYearView.Add(row.Budget, yearView);
                    }
                    else
                    {
                        yearView = (Hashtable)budgetYearView[row.Budget];
                    }

                    if (yearView.Contains(row.Years))
                    {
                        sum = (double)yearView[row.Years];
                        yearView.Remove(row.Years);
                    }
                    sum += cost;
                    yearView.Add(row.Years, sum);
                    costs.Add(new CostDetails
                    {
                        Cost = row.Cost_.Value,
                        Years = row.Years,
                        Budget = row.Budget
                    });
                }
                foreach (string item in budgetYearView.Keys)
                {
                    if (!budgetTypes.Contains(item))
                    {
                        budgetYearView.Remove(item);
                    }
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Network or Simulation");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            var budgetReportDetails = new YearlyBudgetAndCost
            {
                BudgetForYear = budgetYearView,
                CostDetails = costs
            };
            return budgetReportDetails;
        }

        public string[] InvestmentData(SimulationModel data)
        {
            var db = new BridgeCareContext();
            var budgetOrder = db.INVESTMENTs.Where(_ => _.SIMULATIONID == data.SimulationId)
                 .Select(_ => _.BUDGETORDER)
                 .First();
            if (budgetOrder == "" || budgetOrder == null)
            {
                throw new Exception("Budget types not found in Investments table for the id : " + data.SimulationId);
            }
            var budgetTypes = budgetOrder.Split(',');
            return budgetTypes;
        }
    }
}