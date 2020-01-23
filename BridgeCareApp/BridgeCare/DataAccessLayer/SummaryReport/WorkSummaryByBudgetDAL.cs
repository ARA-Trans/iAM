using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;

namespace BridgeCare.DataAccessLayer.SummaryReport
{
    public class WorkSummaryByBudgetDAL : IWorkSummaryByBudget
    {
        public IQueryable<WorkSummaryByBudgetModel> GetworkSummaryByBudgetsData(SimulationModel model, BridgeCareContext db)
        {
            throw new NotImplementedException();
        }
    }
}
