using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class TreatmentData : ITreatments
    {
        public TreatmentData()
        {
        }

        public IQueryable<TreatmentScenarioModel> GetTreatment(SimulationModel data, BridgeCareContext db)
        {
            /// Query will fetch all data with data.Simulationid and fill each TreatmentScenarioModel
            /// object from four tables based on a common TreatmentId
            try
            {
                var treatment = db.Treatments
                    .Include(d => d.CONSEQUENCES)
                    .Include(d => d.FEASIBILITY)
                    .Where(p => p.SIMULATIONID == data.SimulationId)
                    .Select(p => new TreatmentScenarioModel()
                    {
                        SimulationId = p.SIMULATIONID,
                        TreatementId = p.TREATMENTID,
                        Treatement = new TreatmentModel
                        {
                            Treatment = p.TREATMENT1,
                            BeforeAny = p.BEFOREANY,
                            BeforeSame = p.BEFORESAME,
                            Budget = p.BUDGET,
                            Description = p.DESCRIPTION,
                            OMS_IS_EXCLUSIVE = p.OMS_IS_EXCLUSIVE,
                            OMS_IS_REPEAT = p.OMS_IS_REPEAT,
                            OMS_REPEAT_START = p.OMS_REPEAT_START,
                            OMS_REPEAT_INTERVAL = p.OMS_REPEAT_INTERVAL,
                        },
                        Feasibilities = p.FEASIBILITY.Select(m => new FeasibilityModel
                        {
                            Criteria = m.CRITERIA,
                            FeasibilityId = m.FEASIBILITYID,
                        }).ToList(),
                        Consequences = p.CONSEQUENCES.Select(n => new ConsequenceModel
                        {
                            ConsequenceId = n.CONSEQUENCEID,
                            Criteria = n.CRITERIA,
                            Attribute_ = n.ATTRIBUTE_,
                            Change = n.CHANGE_,
                            IsFunction = n.ISFUNCTION,
                            Equation = n.EQUATION,
                        }).ToList()
                    }).ToList();

                foreach (TreatmentScenarioModel t in treatment)
                {
                    EntityClasses.COST cost= db.Costs.SingleOrDefault(p => p.TREATMENTID == t.TreatementId);
                    if (cost == null) continue;
                    t.Cost = new CostModel()
                    {
                        TreatmentId = cost.TREATMENTID,
                        Cost = cost.COST_,
                        CostId = cost.COSTID,
                        Criteria = cost.CRITERIA,
                        IsFunction = cost.ISFUNCTION,
                        Unit = cost.UNIT
                    };
                           
                }

                return treatment.AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Get Treatment Failed");
            }
            return Enumerable.Empty<TreatmentScenarioModel>().AsQueryable();
        }
    }
}