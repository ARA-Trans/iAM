using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace BridgeCare.Models
{
    public class InvestmentLibraryBudgetYearModel
    {
        public InvestmentLibraryBudgetYearModel()
        {
            Budget = new List<InvestmentLibraryBudgetModel>();
        }

        public string Id { get; set; }
        public int Year { get; set; }
        public double? BudgetAmount { get; set; }
        public string BudgetName { get; set; }
        public List<InvestmentLibraryBudgetModel> Budget { get; set; }
    }
}