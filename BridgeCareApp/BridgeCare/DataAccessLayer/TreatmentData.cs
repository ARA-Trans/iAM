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

        public TreatmentsModel GetTreatments(int simulationID, BridgeCareContext db)
        {
            /// Query will fetch all data with data.Simulationid and fill each TreatmentScenarioModel
            /// object from four tables based on a common TreatmentId
            try
            {
                var treatment = db.SIMULATIONS
                    .Include("TREATMENTS")
                    .Include("CONSEQUENCES")
                    .Include("FEASIBILITY")
                    .Include("COST")
                    .Where(b => b.SIMULATIONID == simulationID)
                    .Select(b => new TreatmentsModel()
                    {
                        SimulationId = b.SIMULATIONID,
                        Description = b.COMMENTS,
                        Name = b.SIMULATION1,
                        Treatments = b.TREATMENTS.Select(p => new TreatmentScenarioModel
                        {
                            SimulationId = b.SIMULATIONID,
                            TreatmentId = p.TREATMENTID,
                            Name = p.TREATMENT1,
                            Budget = p.BUDGET,
                            Description = p.DESCRIPTION,
                            OMS_IS_EXCLUSIVE = p.OMS_IS_EXCLUSIVE,
                            OMS_IS_REPEAT = p.OMS_IS_REPEAT,
                            OMS_REPEAT_START = p.OMS_REPEAT_START,
                            OMS_REPEAT_INTERVAL = p.OMS_REPEAT_INTERVAL,
                            Cost = p.COST.Select(q => new CostModel
                            {
                                TreatmentId = p.TREATMENTID,
                                Cost = q.COST_,
                                CostId = q.COSTID,
                                Criteria = q.CRITERIA,
                                IsFunction = q.ISFUNCTION,
                                Unit = q.UNIT
                            }).ToList(),
                            Feasibilities = p.FEASIBILITY.Select(m => new FeasibilityModel
                            {
                                TreatmentId = p.TREATMENTID,
                                Criteria = m.CRITERIA,
                                FeasibilityId = m.FEASIBILITYID,
                                BeforeAny = p.BEFOREANY,
                                BeforeSame = p.BEFORESAME
                            }).ToList(),
                            Consequences = p.CONSEQUENCES.Select(n => new ConsequenceModel
                            {
                                TreatmentId = p.TREATMENTID,
                                ConsequenceId = n.CONSEQUENCEID,
                                Criteria = n.CRITERIA,
                                Attribute_ = n.ATTRIBUTE_,
                                Change = n.CHANGE_,
                                IsFunction = n.ISFUNCTION,
                                Equation = n.EQUATION,
                            }).ToList()
                        }).ToList()
                    }).SingleOrDefault();

                foreach (TreatmentScenarioModel treatmentScenario in treatment.Treatments)
                {
                    treatmentScenario.SetBudgets();
                    if (treatmentScenario.Feasibilities.Count() == 0)
                    {
                        treatmentScenario.Feasilbility = null;
                        continue;
                    }

                    treatmentScenario.Feasilbility = new FeasibilityModel();

                    foreach (FeasibilityModel feasibility in treatmentScenario.Feasibilities)
                    {
                        treatmentScenario.Feasilbility.Agregate(feasibility);
                        treatmentScenario.Feasilbility.BeforeAny = treatmentScenario.BeforeAny;
                        treatmentScenario.Feasilbility.BeforeSame = treatmentScenario.BeforeSame;
                        treatmentScenario.Feasilbility.TreatmentId = treatmentScenario.TreatmentId;
                    }
                }
                return treatment;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Get Treatments Failed");
            }
            return new TreatmentsModel();
        }

        public TreatmentScenarioModel GetTreatment(int treatmentID, BridgeCareContext db)
        {
            /// Query will fetch all data with a treatmentId and fill the TreatmentScenarioModel
            /// object from four tables
            try
            {
                var treatment = db.Treatments
                    .Include("CONSEQUENCES")
                    .Include("FEASIBILITY")
                    .Include("COST")
                    .Where(p => p.TREATMENTID == treatmentID)
                    .Select(p => new TreatmentScenarioModel()
                    {
                        SimulationId = p.SIMULATIONID,
                        TreatmentId = p.TREATMENTID,
                        Name = p.TREATMENT1,
                        Budget = p.BUDGET,
                        Description = p.DESCRIPTION,
                        OMS_IS_EXCLUSIVE = p.OMS_IS_EXCLUSIVE,
                        OMS_IS_REPEAT = p.OMS_IS_REPEAT,
                        OMS_REPEAT_START = p.OMS_REPEAT_START,
                        OMS_REPEAT_INTERVAL = p.OMS_REPEAT_INTERVAL,
                        Cost = p.COST.Select(q => new CostModel
                        {
                            TreatmentId = p.TREATMENTID,
                            Cost = q.COST_,
                            CostId = q.COSTID,
                            Criteria = q.CRITERIA,
                            IsFunction = q.ISFUNCTION,
                            Unit = q.UNIT
                        }).ToList(),
                        Feasibilities = p.FEASIBILITY.Select(m => new FeasibilityModel
                        {
                            TreatmentId = p.TREATMENTID,
                            Criteria = m.CRITERIA,
                            FeasibilityId = m.FEASIBILITYID,
                            BeforeAny = p.BEFOREANY,
                            BeforeSame = p.BEFORESAME
                        }).ToList(),
                        Consequences = p.CONSEQUENCES.Select(n => new ConsequenceModel
                        {
                            TreatmentId = p.TREATMENTID,
                            ConsequenceId = n.CONSEQUENCEID,
                            Criteria = n.CRITERIA,
                            Attribute_ = n.ATTRIBUTE_,
                            Change = n.CHANGE_,
                            IsFunction = n.ISFUNCTION,
                            Equation = n.EQUATION,
                        }).ToList()
                    }).SingleOrDefault();

                treatment.SetBudgets();
                if (treatment.Feasibilities.Count() == 0)
                {
                    treatment.Feasilbility = null;
                }
                else
                {
                    treatment.Feasilbility = new FeasibilityModel();

                    foreach (FeasibilityModel feasibility in treatment.Feasibilities)
                    {
                        treatment.Feasilbility.Agregate(feasibility);
                        treatment.Feasilbility.BeforeAny = treatment.BeforeAny;
                        treatment.Feasilbility.BeforeSame = treatment.BeforeSame;
                        treatment.Feasilbility.TreatmentId = treatment.TreatmentId;
                    }
                }

                return treatment;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Get Treatment Failed");
            }
            return new TreatmentScenarioModel();
        }

        public int CreateTreatment(TreatmentScenarioModel data, BridgeCareContext db)
        {
            var treatment = new TREATMENT()
            {
                SIMULATIONID = data.SimulationId,
                TREATMENTID = 0,
                TREATMENT1 = data.Name,
                BEFOREANY = data.Feasilbility.BeforeAny,
                BEFORESAME = data.Feasilbility.BeforeSame,
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
                    FEASIBILITYID = 0,
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
                        COSTID = 0,
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
                        CONSEQUENCEID = 0,
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

            return treatment.TREATMENTID;
        }

        public TreatmentsModel UpsertTreatment(TreatmentsModel requestedModel, BridgeCareContext db)
        {
            try
            {
                var existingSimulation = db.SIMULATIONS.FirstOrDefault(p => p.SIMULATIONID == requestedModel.SimulationId);

                var existingTreatments = db.Treatments
                    .Include("CONSEQUENCES")
                    .Include("FEASIBILITY")
                    .Include("COST")
                    .Where(p => p.SIMULATIONID == requestedModel.SimulationId).ToList();

                existingSimulation.COMMENTS = requestedModel.Description;
                existingSimulation.SIMULATION1 = requestedModel.Name;

                // the treatment tables get deleted index by index and when they do all the records in
                // sub tables also get deleted, the records in the sub tables are tied to one treatment.
                // if a cost or consequence record is deleted all the records for the particular treatment 
                // shift up and occupy the lower Id's
                foreach (TREATMENT existingTreatment in existingTreatments.ToList())
                {
                    TreatmentScenarioModel treatment = requestedModel.Treatments.SingleOrDefault(t => t.TreatmentId == existingTreatment.TREATMENTID);

                    if (treatment == null)
                    {
                        TREATMENT.delete(existingTreatment, db);
                        continue;
                    }
                    treatment.matched = true;

                    existingTreatment.SIMULATIONID = treatment.SimulationId;
                    existingTreatment.TREATMENTID = treatment.TreatmentId;
                    existingTreatment.TREATMENT1 = treatment.Name;
                    existingTreatment.DESCRIPTION = treatment.Description;

                    // on the database side feasibilties is an array, on the UI side it can be and is
                    // treated as a single record consiting of a criteria. So the DB -> UI sie gets
                    // the array and concatenates it. the UI insert or update wipes out all but one
                    // element of the array
                    if (treatment.Feasilbility != null)
                    {
                        if (existingTreatment.FEASIBILITY == null)
                        {
                            existingTreatment.FEASIBILITY = new List<FEASIBILITY>();
                            existingTreatment.FEASIBILITY.Add(new FEASIBILITY());

                            FEASIBILITY feasibility = existingTreatment.FEASIBILITY.FirstOrDefault();

                            feasibility.TREATMENTID = treatment.TreatmentId;
                            feasibility.FEASIBILITYID = 0;
                            feasibility.CRITERIA = treatment.Feasilbility.Criteria;

                            existingTreatment.BEFOREANY = treatment.Feasilbility.BeforeAny;
                            existingTreatment.BEFORESAME = treatment.Feasilbility.BeforeSame;
                        }
                        else
                        {
                            FEASIBILITY feasibility = existingTreatment.FEASIBILITY.FirstOrDefault();

                            feasibility.TREATMENTID = treatment.TreatmentId;
                            feasibility.CRITERIA = treatment.Feasilbility.Criteria;
                            existingTreatment.BEFOREANY = treatment.Feasilbility.BeforeAny;
                            existingTreatment.BEFORESAME = treatment.Feasilbility.BeforeSame;
                        }
                    }

                    UpsertConsequences(treatment, existingTreatment, db);

                    UpsertCost(treatment, existingTreatment, db);

                    if (treatment.Budgets.Count() > 0)
                    {
                        existingTreatment.BUDGET = treatment.GetBudgets();
                    }
                    else
                    {
                        db.Entry(existingTreatment.BUDGET).State = EntityState.Deleted;
                    }
                }
                foreach (TreatmentScenarioModel treatment in requestedModel.Treatments)
                {
                    if (!treatment.matched)
                    {
                        int treatmentId = CreateTreatment(treatment, db);
                    }
                }

                db.SaveChanges();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Update/Insert Treatment Failed");
            }

            return GetTreatments(requestedModel.SimulationId, db);
        }

        public void UpsertCost(TreatmentScenarioModel treatment,
            TREATMENT existingTreatment, BridgeCareContext db)
        {
            int dataIndex = 0;

            foreach (COST cost in existingTreatment.COST.ToList())
            {
                if (treatment.Cost.Count() > dataIndex)
                {
                    cost.COST_ = treatment.Cost[dataIndex].Cost;
                    cost.CRITERIA = treatment.Cost[dataIndex].Criteria;
                    cost.ISFUNCTION = treatment.Cost[dataIndex].IsFunction;
                    cost.UNIT = treatment.Cost[dataIndex].Unit;
                }
                else
                {
                    db.Entry(cost).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the number of updated records exceeds the number of existing records
            while (treatment.Cost.Count() > dataIndex)
            {
                var costEntity = new COST()
                {
                    COSTID = 0,
                    COST_ = treatment.Cost[dataIndex].Cost,
                    CRITERIA = treatment.Cost[dataIndex].Criteria,
                    ISFUNCTION = treatment.Cost[dataIndex].IsFunction,
                    UNIT = treatment.Cost[dataIndex].Unit,
                    TREATMENTID = treatment.TreatmentId
                };
                existingTreatment.COST.Add(costEntity);
                dataIndex++;
            }
        }

        public void UpsertConsequences(TreatmentScenarioModel treatment,
            TREATMENT existingTreatment, BridgeCareContext db)
        {
            int dataIndex = 0;

            foreach (CONSEQUENCE consequence in existingTreatment.CONSEQUENCES.ToList())
            {
                if (treatment.Consequences.Count() > dataIndex)
                {
                    consequence.EQUATION = treatment.Consequences[dataIndex].Equation;
                    consequence.CRITERIA = treatment.Consequences[dataIndex].Criteria;
                    consequence.ATTRIBUTE_ = treatment.Consequences[dataIndex].Attribute_;
                    consequence.CHANGE_ = treatment.Consequences[dataIndex].Change;
                    consequence.ISFUNCTION = treatment.Consequences[dataIndex].IsFunction;
                }
                else
                {
                    db.Entry(consequence).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the updated records exceed existing records
            while (treatment.Consequences.Count() > dataIndex)
            {
                var consequenceEntity = new CONSEQUENCE()
                {
                    TREATMENTID = treatment.TreatmentId,
                    EQUATION = treatment.Consequences[dataIndex].Equation,
                    CRITERIA = treatment.Consequences[dataIndex].Criteria,
                    ATTRIBUTE_ = treatment.Consequences[dataIndex].Attribute_,
                    CHANGE_ = treatment.Consequences[dataIndex].Change,
                    ISFUNCTION = treatment.Consequences[dataIndex].IsFunction,
                    CONSEQUENCEID = 0
                };
                existingTreatment.CONSEQUENCES.Add(consequenceEntity);
                dataIndex++;
            }
        }
    }
}