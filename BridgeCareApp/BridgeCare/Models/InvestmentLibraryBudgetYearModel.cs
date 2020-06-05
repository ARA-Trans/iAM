using System.Collections.Generic;
using BridgeCare.EntityClasses;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace BridgeCare.Models
{
    public class InvestmentLibraryBudgetYearModel : CrudModel
    {
        public string Id { get; set; }
        public int Year { get; set; }
        public string BudgetName { get; set; }
        public double? BudgetAmount { get; set; }
        public string CriteriaDrivenBudgetId { get; set; }

        public InvestmentLibraryBudgetYearModel() { }

        public InvestmentLibraryBudgetYearModel(YearlyInvestmentEntity entity)
        {
            Id = entity.YEARID.ToString();
            Year = entity.YEAR_;
            BudgetName = entity.BUDGET_CRITERIA.BUDGET_NAME;
            CriteriaDrivenBudgetId = entity.BUDGET_CRITERIA_ID.ToString();
            BudgetAmount = entity.AMOUNT;
        }

        public void UpdateYearlyInvestment(YearlyInvestmentEntity entity)
        {
            entity.AMOUNT = BudgetAmount;
        }
    }
}
