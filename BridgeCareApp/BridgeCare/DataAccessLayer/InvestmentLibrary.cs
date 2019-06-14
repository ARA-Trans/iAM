using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class InvestmentLibrary : IInvestmentLibrary
    {
        private readonly BridgeCareContext db;

        public InvestmentLibrary(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public InvestmentLibraryModel GetScenarioInvestmentLibrary(int selectedScenarioId, BridgeCareContext db)
        {
            try
            {
                var investmentLibraryModel = db.SIMULATIONS
                    .Include(d => d.INVESTMENTS)
                    .Include(d => d.YEARLYINVESTMENTs)
                    .Where(d => d.SIMULATIONID == selectedScenarioId)
                    .Select(p => new InvestmentLibraryModel()
                    {
                        Name = p.SIMULATION1,
                        Description = p.COMMENTS,
                        Id = p.SIMULATIONID,
                        NetworkId = p.NETWORKID ?? 0,
                        FirstYear = p.INVESTMENTS.FIRSTYEAR ?? 0,
                        NumberYears = p.INVESTMENTS.NUMBERYEARS ?? 0,
                        InflationRate = p.INVESTMENTS.INFLATIONRATE ?? 0,
                        DiscountRate = p.INVESTMENTS.DISCOUNTRATE ?? 0,
                        BudgetNamesByOrder = p.INVESTMENTS.BUDGETORDER,
                        BudgetYears = p.YEARLYINVESTMENTs.Select(m => new InvestmentLibraryBudgetYearModel
                        {
                            Id = m.YEARID.ToString(),
                            Year = m.YEAR_,
                            BudgetAmount = m.AMOUNT,
                            BudgetName = m.BUDGETNAME
                        }).ToList()
                    }).FirstOrDefault();

                investmentLibraryModel.SetBudgets();
                return investmentLibraryModel;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Scenario Investment Library");
            }
            return new InvestmentLibraryModel();
        }

        public InvestmentLibraryModel SaveScenarioInvestmentLibrary(InvestmentLibraryModel data, BridgeCareContext db)
        {
            // Ensures budget order is transferred from array storage as it comes in from json to the
            // databse format, comma delimited
            string budgetNamesByOrder = data.GetBudgetOrder();

            // Derive FirstYear and NumberYears from the YearlyBudget list.
            data.FirstYear = data.BudgetYears.Min(r => r.Year);
            data.NumberYears = data.BudgetYears.Max(r => r.Year) - data.FirstYear;

            try
            {
                var simulation = db.SIMULATIONS
                    .Include(d => d.INVESTMENTS)
                    .Include(d => d.YEARLYINVESTMENTs)
                    .Single(_ => _.SIMULATIONID == data.Id);

                simulation.COMMENTS = data.Description;
                simulation.SIMULATION1 = data.Name;
                simulation.INVESTMENTS.FIRSTYEAR = data.FirstYear;
                simulation.INVESTMENTS.NUMBERYEARS = data.NumberYears;
                simulation.INVESTMENTS.INFLATIONRATE = data.InflationRate;
                simulation.INVESTMENTS.DISCOUNTRATE = data.DiscountRate;
                simulation.INVESTMENTS.BUDGETORDER = budgetNamesByOrder;

                UpsertYearlyData(data, simulation, db);

                db.SaveChanges();
                return data;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Scenario Investment Library");
            }
            // Returning empty model in case of any exception.
            return new InvestmentLibraryModel();
        }
        public void UpsertYearlyData(InvestmentLibraryModel investment,
           SIMULATION simulation, BridgeCareContext db)
        {
            int dataIndex = 0;

            foreach (YEARLYINVESTMENT yearlyInvestment in simulation.YEARLYINVESTMENTs.ToList())
            {
                if (investment.BudgetYears.Count() > dataIndex)
                {
                    yearlyInvestment.YEAR_ = investment.BudgetYears[dataIndex].Year;
                    yearlyInvestment.BUDGETNAME= investment.BudgetYears[dataIndex].BudgetName;
                    yearlyInvestment.AMOUNT = investment.BudgetYears[dataIndex].BudgetAmount;
                }
                else
                {
                    db.Entry(yearlyInvestment).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the updated records exceed existing records
            while (investment.BudgetYears.Count() > dataIndex)
            {
                var yearly = new YEARLYINVESTMENT(investment.Id,
                    investment.BudgetYears[dataIndex].Year,
                    investment.BudgetYears[dataIndex].BudgetName,
                    investment.BudgetYears[dataIndex].BudgetAmount);

                simulation.YEARLYINVESTMENTs.Add(yearly);
                dataIndex++;
            }
        }
    }
}