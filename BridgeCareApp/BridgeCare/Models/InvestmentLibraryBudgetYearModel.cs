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
        public List<InvestmentLibraryBudgetModel> Budget { get; set; }

        public InvestmentLibraryBudgetYearModel()
        {
            Budget = new List<InvestmentLibraryBudgetModel>();
        }

        public InvestmentLibraryBudgetYearModel(YearlyInvestmentEntity entity)
        {
            Id = entity.YEARID.ToString();
            Year = entity.YEAR_;
            BudgetName = entity.BUDGETNAME;
            BudgetAmount = entity.AMOUNT;
        }

        public void UpdateYearlyInvestment(YearlyInvestmentEntity entity)
        {
            entity.YEAR_ = Year;
            entity.AMOUNT = BudgetAmount;
            entity.BUDGETNAME = BudgetName;
        }
    }
}