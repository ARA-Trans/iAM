using BridgeCareCodeFirst.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.ApplicationLog;

namespace BridgeCare.Models
{
    public class BudgetReportData : IBudgetReport
    {
        private readonly List<ICostDetails> costs;
        private readonly BridgeCareContext db;

        public BudgetReportData(List<ICostDetails> cost, BridgeCareContext context)
        {
            costs = cost ?? throw new ArgumentNullException(nameof(cost));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }
        public BudgetReportDetails GetBudgetReportData(SimulationResult data, string[] budgetTypes)
        {
            var BudgetYearView = new Hashtable();
            var select =
                "SELECT Years, Budget, Cost_ " +
                    " FROM Report_" + data.NetworkId
                    + "_" + data.SimulationId + " WHERE BUDGET is not null";
            try
            {
                var rawQueryForData = db.Database.SqlQuery<BudgetReport>(select).AsQueryable();

                double sum = 0;
                foreach (var row in rawQueryForData)
                {
                    double.TryParse(row.Cost_.Value.ToString(), out double cost);
                    Hashtable yearView;
                    //[NOTE].. Filling up the hash table. Data is coming from the database in such a way
                    // that an Hashtable cannot be filled in one go and the checks are necessary.
                    if (!BudgetYearView.Contains(row.Budget))
                    {
                        yearView = new Hashtable();
                        BudgetYearView.Add(row.Budget, yearView);
                    }
                    else
                    {
                        yearView = (Hashtable)BudgetYearView[row.Budget];
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
                foreach (string item in BudgetYearView.Keys)
                {
                    if (!budgetTypes.Contains(item))
                    {
                        BudgetYearView.Remove(item);
                    }
                }
            }
            catch (SqlException ex)
            {
                ThrowError.SqlError(ex, "Network or Simulation");
            }
            catch (OutOfMemoryException ex)
            {
                ThrowError.OutOfMemoryError(ex);
            }
            var budgetReportDetails = new BudgetReportDetails
            {
                BudgetForYear = BudgetYearView,
                CostDetails = costs
            };
            return budgetReportDetails;
        }

        public string[] InvestmentData(SimulationResult data)
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

        public class CostDetails : ICostDetails
        {
            public double Cost { get; set; }
            public int Years { get; set; }
            public string Budget { get; set; }
        }

        public class BudgetReportDetails
        {
            public Hashtable BudgetForYear;
            public List<ICostDetails> CostDetails;
        }
    }
}