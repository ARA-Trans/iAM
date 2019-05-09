using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Data;
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

        public IQueryable<TreatmentScenarioModel> GetTreatments(int simulationID, BridgeCareContext db)
        {
            /// Query will fetch all data with data.Simulationid and fill each TreatmentScenarioModel
            /// object from four tables based on a common TreatmentId
            try
            {
                var treatment = db.Treatments
                    .Include("CONSEQUENCES")
                    .Include("FEASIBILITY")
                    .Include("COST")
                    .Where(p => p.SIMULATIONID == simulationID)
                    .Select(p => new TreatmentScenarioModel()
                    {
                        SimulationId = p.SIMULATIONID,
                        TreatementId = p.TREATMENTID,
                        Treatment = p.TREATMENT1,
                        BeforeAny = p.BEFOREANY,
                        BeforeSame = p.BEFORESAME,
                        Budget = p.BUDGET,
                        Description = p.DESCRIPTION,
                        OMS_IS_EXCLUSIVE = p.OMS_IS_EXCLUSIVE,
                        OMS_IS_REPEAT = p.OMS_IS_REPEAT,
                        OMS_REPEAT_START = p.OMS_REPEAT_START,
                        OMS_REPEAT_INTERVAL = p.OMS_REPEAT_INTERVAL,
                        Cost = p.COST.Select(q => new CostModel
                        {
                            Cost = q.COST_,
                            CostId = q.COSTID,
                            Criteria = q.CRITERIA,
                            IsFunction = q.ISFUNCTION,
                            Unit = q.UNIT
                        }).ToList(),
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

                foreach (TreatmentScenarioModel treatmentScenario in treatment)
                {
                    treatmentScenario.Feasilbility = new FeasibilityModel();
                    treatmentScenario.SetBudgets();

                    foreach (FeasibilityModel feasibility in treatmentScenario.Feasibilities)
                    {
                        treatmentScenario.Feasilbility.Agregate(feasibility);
                    }
                }
                return treatment.AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Get Treatments Failed");
            }
            return Enumerable.Empty<TreatmentScenarioModel>().AsQueryable();
        }

        public TreatmentScenarioModel GetTreatment(int treatmentID, BridgeCareContext db)
        {
            /// Query will fetch all data with data.Simulationid and fill each TreatmentScenarioModel
            /// object from four tables based on a common TreatmentId
            try
            {
                var treatment = db.Treatments
                    .Include("CONSEQUENCES")
                    .Include("FEASIBILITY")
                    .Include("COST")
                    .SingleOrDefault(p => p.TREATMENTID == treatmentID);

                TreatmentScenarioModel treatmentScenario = new TreatmentScenarioModel()
                {
                    SimulationId = treatment.SIMULATIONID,
                    TreatementId = treatment.TREATMENTID,
                    Treatment = treatment.TREATMENT1,
                    BeforeAny = treatment.BEFOREANY,
                    BeforeSame = treatment.BEFORESAME,
                    Budget = treatment.BUDGET,
                    Description = treatment.DESCRIPTION,
                    OMS_IS_EXCLUSIVE = treatment.OMS_IS_EXCLUSIVE,
                    OMS_IS_REPEAT = treatment.OMS_IS_REPEAT,
                    OMS_REPEAT_START = treatment.OMS_REPEAT_START,
                    OMS_REPEAT_INTERVAL = treatment.OMS_REPEAT_INTERVAL,
                    Cost = treatment.COST.Select(q => new CostModel
                    {
                        Cost = q.COST_,
                        CostId = q.COSTID,
                        Criteria = q.CRITERIA,
                        IsFunction = q.ISFUNCTION,
                        Unit = q.UNIT
                    }).ToList(),
                    Feasibilities = treatment.FEASIBILITY.Select(m => new FeasibilityModel
                    {
                        Criteria = m.CRITERIA,
                        FeasibilityId = m.FEASIBILITYID,
                    }).ToList(),
                    Consequences = treatment.CONSEQUENCES.Select(n => new ConsequenceModel
                    {
                        ConsequenceId = n.CONSEQUENCEID,
                        Criteria = n.CRITERIA,
                        Attribute_ = n.ATTRIBUTE_,
                        Change = n.CHANGE_,
                        IsFunction = n.ISFUNCTION,
                        Equation = n.EQUATION,
                    }).ToList()
                };

                treatmentScenario.Feasilbility = new FeasibilityModel();
                treatmentScenario.SetBudgets();

                foreach (FeasibilityModel feasibility in treatmentScenario.Feasibilities)
                {
                    treatmentScenario.Feasilbility.Agregate(feasibility);
                }

                return treatmentScenario;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Get Treatment Failed");
            }
            return new TreatmentScenarioModel();
        }

        public TreatmentScenarioModel CreateTreatment(TreatmentScenarioModel data, BridgeCareContext db)
        {
            var treatment = new TREATMENT()
            {
                SIMULATIONID = data.SimulationId,
                TREATMENTID = 0,
                TREATMENT1 = data.Treatment,
                BEFOREANY = data.BeforeAny,
                BEFORESAME = data.BeforeSame,
                DESCRIPTION = data.Description,
                OMS_IS_EXCLUSIVE = data.OMS_IS_EXCLUSIVE,
                OMS_IS_REPEAT = data.OMS_IS_REPEAT,
                OMS_REPEAT_START = data.OMS_REPEAT_INTERVAL,
                OMS_REPEAT_INTERVAL = data.OMS_REPEAT_START,
                CONSEQUENCES = null,
                COST = null,
                SCHEDULED = null
            };

            treatment.FEASIBILITY = new List<FEASIBILITY>()
            {
                new FEASIBILITY()
                {
                    TREATMENTID = 0,
                    CRITERIA = data.Feasilbility.Criteria
                }
            };

            if (data.Cost.Count() > 0)
            {
                treatment.COST = new List<COST>();

                foreach (CostModel cost in data.Cost)
                {
                    var costEntity = new COST()
                    {
                        TREATMENTID = 0,
                        COST_ = cost.Cost,
                        CRITERIA = cost.Criteria,
                        ISFUNCTION = cost.IsFunction,
                        UNIT = cost.Unit
                    };
                    treatment.COST.Add(costEntity);
                }
            }

            if (data.Consequences.Count() > 0)
            {
                treatment.CONSEQUENCES = new List<CONSEQUENCE>();
                foreach (ConsequenceModel consequence in data.Consequences)
                {
                    var consequenceEntity = new CONSEQUENCE()
                    {
                        TREATMENTID = 0,
                        EQUATION = consequence.Equation,
                        CRITERIA = consequence.Criteria,
                        ATTRIBUTE_ = consequence.Attribute_,
                        CHANGE_ = consequence.Change,
                        ISFUNCTION = consequence.IsFunction
                    };
                    treatment.CONSEQUENCES.Add(consequenceEntity);
                }
            }
            if (data.Budgets.Count() > 0)
            {
                treatment.BUDGET = data.GetBudgets();
            }

            db.Treatments.Add(treatment);

            db.SaveChanges();

            return GetTreatment(treatment.TREATMENTID, db);
        }

        public TreatmentScenarioModel UpsertTreatment(TreatmentScenarioModel data, BridgeCareContext db)
        {
            // if a primary key is zero for any table then it is considered an insert if not it is an
            // update if a key is missing in the passed in data but exists in the database it is
            // considered a delete
            if (data.TreatementId == 0)
            {
                return CreateTreatment(data, db);
            }

            var existingTreatment = db.Treatments
                .Include("CONSEQUENCES")
                .Include("FEASIBILITY")
                .Include("COST")
                .SingleOrDefault(p => p.TREATMENTID == data.TreatementId);

            existingTreatment.SIMULATIONID = data.SimulationId;
            existingTreatment.TREATMENTID = data.TreatementId;
            existingTreatment.TREATMENT1 = data.Treatment;
            existingTreatment.BEFOREANY = data.BeforeAny;
            existingTreatment.BEFORESAME = data.BeforeSame;
            existingTreatment.DESCRIPTION = data.Description;
            existingTreatment.OMS_IS_EXCLUSIVE = data.OMS_IS_EXCLUSIVE;
            existingTreatment.OMS_IS_REPEAT = data.OMS_IS_REPEAT;
            existingTreatment.OMS_REPEAT_START = data.OMS_REPEAT_INTERVAL;
            existingTreatment.OMS_REPEAT_INTERVAL = data.OMS_REPEAT_START;

            if (data.Feasilbility != null)
            {
                if (existingTreatment.FEASIBILITY == null)
                {
                    existingTreatment.FEASIBILITY = new List<FEASIBILITY>();
                    existingTreatment.FEASIBILITY.Add(new FEASIBILITY());

                    FEASIBILITY feasibility = existingTreatment.FEASIBILITY.FirstOrDefault();

                    feasibility.TREATMENTID = data.TreatementId;
                    feasibility.FEASIBILITYID = 0;
                    feasibility.CRITERIA = data.Feasilbility.Criteria;
                }
                else
                {
                    FEASIBILITY feasibility = existingTreatment.FEASIBILITY.FirstOrDefault();

                    feasibility.TREATMENTID = data.TreatementId;
                    feasibility.CRITERIA = data.Feasilbility.Criteria;
                }
            }

            int dataIndex = 0;

            foreach (COST cost in existingTreatment.COST)
            {
                if (data.Cost.Count() > dataIndex)
                {
                    cost.COST_ = data.Cost[dataIndex].Cost;
                    cost.CRITERIA = data.Cost[dataIndex].Criteria;
                    cost.ISFUNCTION = data.Cost[dataIndex].IsFunction;
                    cost.UNIT = data.Cost[dataIndex].Unit;
                }
                else
                {
                    db.Entry(cost).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the updated records exceed existing records
            while (data.Cost.Count() > dataIndex)
            {
                var costEntity = new COST()
                {
                    COSTID = 0,
                    COST_ = data.Cost[dataIndex].Cost,
                    CRITERIA = data.Cost[dataIndex].Criteria,
                    ISFUNCTION = data.Cost[dataIndex].IsFunction,
                    UNIT = data.Cost[dataIndex].Unit,
                    TREATMENTID = data.TreatementId
                };
                existingTreatment.COST.Add(costEntity);
                dataIndex++;
            }

            dataIndex = 0;

            foreach (CONSEQUENCE consequence in existingTreatment.CONSEQUENCES)
            {
                if (data.Consequences.Count() > dataIndex)
                {
                    consequence.EQUATION = data.Consequences[dataIndex].Equation;
                    consequence.CRITERIA = data.Consequences[dataIndex].Criteria;
                    consequence.ATTRIBUTE_ = data.Consequences[dataIndex].Attribute_;
                    consequence.CHANGE_ = data.Consequences[dataIndex].Change;
                    consequence.ISFUNCTION = data.Consequences[dataIndex].IsFunction;
                }
                else
                {
                    db.Entry(consequence).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the updated records exceed existing records
            while (data.Consequences.Count() > dataIndex)
            {
                var consequenceEntity = new CONSEQUENCE()
                {
                    TREATMENTID = data.TreatementId,
                    EQUATION = data.Consequences[dataIndex].Equation,
                    CRITERIA = data.Consequences[dataIndex].Criteria,
                    ATTRIBUTE_ = data.Consequences[dataIndex].Attribute_,
                    CHANGE_ = data.Consequences[dataIndex].Change,
                    ISFUNCTION = data.Consequences[dataIndex].IsFunction,
                    CONSEQUENCEID = 0
                };
                existingTreatment.CONSEQUENCES.Add(consequenceEntity);
                dataIndex++;
            }
            if (data.Budgets.Count() > 0)
            {
                existingTreatment.BUDGET = data.GetBudgets();
            }
            else
            {
                db.Entry(existingTreatment.BUDGET).State = EntityState.Deleted;
            }

            db.SaveChanges();

            return GetTreatment(existingTreatment.TREATMENTID, db);
        }
    }
}