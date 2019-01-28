using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using static BridgeCare.Models.BudgetReportData;

namespace BridgeCare.Models
{
    public class BudgetReportData : IBudgetReportData
    {
        private readonly List<ICostDetails> costList;

        public BudgetReportData()
        {
        }
        public BudgetReportData(List<ICostDetails> costList)
        {
            this.costList = costList ?? throw new ArgumentNullException(nameof(costList));
        }
        public BudgetReportDetails GetBudgetReportData(SimulationResult data, string[] budgetTypes)
        {
            var BudgetYearView = new Hashtable();
            var db = new BridgeCareContext();
            string getReport =
                "SELECT Years, Budget, Cost_ " +
                    " FROM Report_" + data.NetworkId
                    + "_" + data.SimulationId + " WHERE BUDGET is not null";
            try
            {
                var RawQueryForData = db.Database.SqlQuery<BudgetReport>(getReport).AsQueryable();

                double sum = 0;
                foreach (var report in RawQueryForData)
                {
                    double.TryParse(report.Cost_.Value.ToString(), out double cost);
                    Hashtable yearView;
                    if (!BudgetYearView.Contains(report.Budget))
                    {
                        yearView = new Hashtable();
                        BudgetYearView.Add(report.Budget, yearView);
                    }
                    else
                    {
                        yearView = (Hashtable)BudgetYearView[report.Budget];
                    }

                    if (yearView.Contains(report.Years))
                    {
                        sum = (double)yearView[report.Years];
                        yearView.Remove(report.Years);
                    }
                    sum += cost;
                    yearView.Add(report.Years, sum);
                    costList.Add(new CostDetails
                    {
                        Cost = report.Cost_.Value,
                        Years = report.Years,
                        Budget = report.Budget
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
                db.Dispose();
                if (ex.Number == -2 || ex.Number == 11)
                {
                    throw new TimeoutException("The server has timed out. Please try after some time");
                }
                if (ex.Number == 208)
                {
                    throw new InvalidOperationException("Network or simulation table does not exist in the database");
                }
            }
            catch (OutOfMemoryException)
            {
                db.Dispose();
                throw new OutOfMemoryException("The server is out of memory. Please try after some time");
            }
            db.Dispose();
            var budgetReportDetails = new BudgetReportDetails
            {
                BudgetForYear = BudgetYearView,
                CostDetails = costList
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
                db.Dispose();
                throw new Exception("Budget types not found in Investments table for the id : " + data.SimulationId);
            }
            var budgetTypes = budgetOrder.Split(',');
            db.Dispose();
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
        public interface ICostDetails
        {
            double Cost { get; set; }
            int Years { get; set; }
            string Budget { get; set; }
        }
        public interface IBudgetReportData
        {
            BudgetReportDetails GetBudgetReportData(SimulationResult data, string[] budgetTypes);
            string[] InvestmentData(SimulationResult data);

        }
    }
}