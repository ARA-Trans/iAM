using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class BudgetReportDAL : IBudgetReport
    {
        private readonly List<CostDetails> costs;
        private readonly BridgeCareContext db;

        public BudgetReportDAL(List<CostDetails> costs, BridgeCareContext db)
        {
            this.costs = costs ?? throw new ArgumentNullException(nameof(costs));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public YearlyBudgetAndCost GetData(SimulationModel data, string[] budgetTypes)
        {
            var budgetForYear = new Hashtable();

            var query = "SELECT Years, Budget, Cost_ " +
                         $"FROM Report_{data.NetworkId}_{data.SimulationId} " +
                         $"WHERE BUDGET is not null AND BUDGET IN ('{string.Join("','",budgetTypes)}')";
            var rawQueryForData = db.Database.SqlQuery<BudgetModel>(query).AsQueryable();

            foreach (var row in rawQueryForData)
            {
                if (!budgetForYear.ContainsKey(row.Budget))
                    budgetForYear.Add(row.Budget, new Hashtable());

                var cost = row.Cost_ ?? 0;

                var yearForCost = (Hashtable)budgetForYear[row.Budget];

                if (!yearForCost.ContainsKey(row.Years))
                    yearForCost.Add(row.Years, cost);
                else
                    yearForCost[row.Years] = (double)yearForCost[row.Years] + cost;

                costs.Add(new CostDetails(row));
            }

            return new YearlyBudgetAndCost(budgetForYear, costs);
        }

        public string[] InvestmentData(SimulationModel model)
        {
            var budgetOrder = db.Investments.Where(investment => investment.SIMULATIONID == model.SimulationId)
                 .Select(investment => investment.BUDGETORDER)
                 .FirstOrDefault();

            if (string.IsNullOrEmpty(budgetOrder))
                throw new Exception("Budget types not found in Investments table for the id : " + model.SimulationId);

            return budgetOrder.Split(',');
        }
    }
}