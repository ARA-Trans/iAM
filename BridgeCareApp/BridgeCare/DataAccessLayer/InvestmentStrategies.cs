using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.DataAccessLayer
{
    public class InvestmentStrategies : IInvestmentStrategies
    {
        private readonly BridgeCareContext db;

        public InvestmentStrategies(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<InvestmentStrategyModel> GetInvestmentStrategies(int simulationId, BridgeCareContext db)
        {
            try
            {
                var simulation = db.SIMULATIONS
                    .Include(d => d.INVESTMENTS)
                    .Include(d => d.YEARLYINVESTMENTs)
                    .Where(d => d.SIMULATIONID == simulationId )
                    .Select(p => new InvestmentStrategyModel()
                    {
                        Name = p.SIMULATION1,
                        Description = p.COMMENTS,
                        SimulationId = p.SIMULATIONID,
                        NetworkId = p.NETWORKID ?? 0,
                        FirstYear = p.INVESTMENTS.FIRSTYEAR ?? 0,
                        NumberYears = p.INVESTMENTS.NUMBERYEARS ?? 0,
                        InflationRate = p.INVESTMENTS.INFLATIONRATE ?? 0,
                        DiscountRate = p.INVESTMENTS.DISCOUNTRATE ?? 0,
                        BudgetOrder = p.INVESTMENTS.BUDGETORDER,
                        YearlyBudgets = p.YEARLYINVESTMENTs.Select(m => new InvestmentStrategyYearlyBudgetModel
                        {
                            Id = m.YEARID,
                            Year = m.YEAR_,
                            BudgetAmount = m.AMOUNT,
                            BudgetName = m.BUDGETNAME
                        }).ToList()
                    }).Where((t) => t.SimulationId == simulationId).ToList();

                foreach (InvestmentStrategyModel model in simulation)
                {
                    model.SetBudgets();
                }

                return simulation.AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Investment Strategies");
            }
            return Enumerable.Empty<InvestmentStrategyModel>().AsQueryable();
        }

        public void SetInvestmentStrategies(InvestmentStrategyModel data, BridgeCareContext db)
        {
            // Ensures budget order is transferred from array storage as it comes in from json to the
            // databse format, comma delimited
            string budgetOrder = data.GetBudgetOrder();

            // Derive FirstYear and NumberYears from the YearlyBudget list.
            data.FirstYear = data.YearlyBudgets.Min(r => r.Year);
            data.NumberYears = data.YearlyBudgets.Max(r => r.Year) - data.FirstYear;

            try
            {
                var simulation = db.SIMULATIONS
                    .Include(d => d.INVESTMENTS)
                    .Include(d => d.YEARLYINVESTMENTs)
                    .Single(_ => _.SIMULATIONID == data.SimulationId);

                simulation.COMMENTS = data.Description;
                simulation.SIMULATION1 = data.Name;
                simulation.INVESTMENTS.FIRSTYEAR = data.FirstYear;
                simulation.INVESTMENTS.NUMBERYEARS = data.NumberYears;
                simulation.INVESTMENTS.INFLATIONRATE = data.InflationRate;
                simulation.INVESTMENTS.DISCOUNTRATE = data.DiscountRate;
                simulation.INVESTMENTS.BUDGETORDER = budgetOrder;

                db.YEARLYINVESTMENTs.RemoveRange(simulation.YEARLYINVESTMENTs);

                List<YEARLYINVESTMENT> investments = new List<YEARLYINVESTMENT>();

                foreach (InvestmentStrategyYearlyBudgetModel year in data.YearlyBudgets)
                {
                    investments.Add(new YEARLYINVESTMENT(data.SimulationId, year.Year, year.BudgetName, year.BudgetAmount));
                }
                db.YEARLYINVESTMENTs.AddRange(investments);

                db.SaveChanges();
                return;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Investment Strategies");
            }
            return;
        }
    }
}