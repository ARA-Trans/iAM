using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using BridgeCare.Interfaces.CriteriaDrivenBudgets;
using BridgeCare.Models.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BridgeCare.DataAccessLayer.CriteriaDrivenBudgets
{
    public class CriteriaDrivenBudgetsDAL : ICriteriaDrivenBudgets
    {
        public List<CriteriaDrivenBudgetsModel> GetCriteriaDrivenBudgets(int simulationId, BridgeCareContext db)
        {
            try
            {
                if (db.CriteriaDrivenBudgets.Any(criteriaBudgets => criteriaBudgets.SIMULATIONID == simulationId))
                {
                    var CriteriaForBudgets = new List<CriteriaDrivenBudgetsModel>();
                    CriteriaForBudgets = db.CriteriaDrivenBudgets.AsNoTracking().Where(_ => _.SIMULATIONID == simulationId)
                                                              .Select(p => new CriteriaDrivenBudgetsModel
                                                              {
                                                                  BudgetCriteriaId = p.BUDGET_CRITERIA_ID,
                                                                  ScenarioId = p.SIMULATIONID,
                                                                  BudgetName = p.BUDGET_NAME,
                                                                  Criteria = p.CRITERIA
                                                              }).ToList();
                    return CriteriaForBudgets;
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "CriteriaDrivenBudgets");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }
            return new List<CriteriaDrivenBudgetsModel>();
        }

        public Task<string> SaveCriteriaDrivenBudgets(int selectedScenarioId, List<CriteriaDrivenBudgetsModel> data, BridgeCareContext db)
        {
            try
            {
                var existingBudgets = db.CriteriaDrivenBudgets.Where(budgets => budgets.SIMULATIONID == selectedScenarioId).ToList();

                db.CriteriaDrivenBudgets.RemoveRange(existingBudgets);
                db.CriteriaDrivenBudgets.AddRange(data.Select(criteriaModel => new CriteriaDrivenBudgetsEntity(criteriaModel)).ToList());

                db.SaveChanges();
                return Task.FromResult("Saved criteria driven budgets");
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "CriteriaDrivenBudgets");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }
            return Task.FromResult("Failed to save criteria driven budgets"); ;
        }
    }
}