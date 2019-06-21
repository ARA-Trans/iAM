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
    public class InvestmentLibraryDAL : IInvestmentLibrary
    {
        private readonly BridgeCareContext db;

        public InvestmentLibraryDAL(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public InvestmentLibraryModel GetScenarioInvestmentLibrary(int selectedScenarioId, BridgeCareContext db)
        {
            try
            {
                var investmentLibraryModel = db.Simulations
                    .Include(d => d.INVESTMENTS)
                    .Include(d => d.YEARLYINVESTMENTS)
                    .Where(d => d.SIMULATIONID == selectedScenarioId)
                    .Select(p => new InvestmentLibraryModel()
                    {
                        Name = p.SIMULATION,
                        Description = p.COMMENTS,
                        Id = p.SIMULATIONID,
                        NetworkId = p.NETWORKID ?? 0,
                        FirstYear = p.INVESTMENTS.FIRSTYEAR ?? 0,
                        NumberYears = p.INVESTMENTS.NUMBERYEARS ?? 0,
                        InflationRate = p.INVESTMENTS.INFLATIONRATE ?? 0,
                        DiscountRate = p.INVESTMENTS.DISCOUNTRATE ?? 0,
                        BudgetNamesByOrder = p.INVESTMENTS.BUDGETORDER,
                        BudgetYears = p.YEARLYINVESTMENTS.Select(m => new InvestmentLibraryBudgetYearModel
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
            data.FirstYear = data.BudgetYears.Any() ? data.BudgetYears.Min(r => r.Year) : DateTime.Now.Year;
            data.NumberYears = data.BudgetYears.Any() ? data.BudgetYears.Max(r => r.Year) - data.FirstYear : 1;

            try
            {
                var simulation = db.Simulations
                    .Include(d => d.INVESTMENTS)
                    .Include(d => d.YEARLYINVESTMENTS)
                    .Single(_ => _.SIMULATIONID == data.Id);

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
           SimulationEntity simulation, BridgeCareContext db)
        {
            int dataIndex = 0;

            foreach (YearlyInvestmentEntity yearlyInvestment in simulation.YEARLYINVESTMENTS.ToList())
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
                var yearly = new YearlyInvestmentEntity(investment.Id,
                    investment.BudgetYears[dataIndex].Year,
                    investment.BudgetYears[dataIndex].BudgetName,
                    investment.BudgetYears[dataIndex].BudgetAmount);

                simulation.YEARLYINVESTMENTS.Add(yearly);
                dataIndex++;
            }
        }
    }
}