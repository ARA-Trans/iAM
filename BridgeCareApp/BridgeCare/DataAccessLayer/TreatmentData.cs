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
            var newTreatmentModel = new TREATMENT()
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

            newTreatmentModel.FEASIBILITY = new List<FEASIBILITY>()
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
                newTreatmentModel.COST = new List<COST>();

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
                    newTreatmentModel.COST.Add(costEntity);
                }
            }

            if (data.Consequences.Count() > 0)
            {
                newTreatmentModel.CONSEQUENCES = new List<CONSEQUENCE>();
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
                    newTreatmentModel.CONSEQUENCES.Add(consequenceEntity);
                }
            }
            if (data.Budgets.Count() > 0)
            {
                newTreatmentModel.BUDGET = data.GetBudgets();
            }

            db.Treatments.Add(newTreatmentModel);

            db.SaveChanges();

            return newTreatmentModel.TREATMENTID;
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
                    TreatmentScenarioModel treatmentModel = requestedModel.Treatments.SingleOrDefault(t => t.TreatmentId == existingTreatment.TREATMENTID);

                    if (treatmentModel == null)
                    {
                        TREATMENT.delete(existingTreatment, db);
                        continue;
                    }
                    treatmentModel.matched = true;

                    existingTreatment.SIMULATIONID = treatmentModel.SimulationId;
                    existingTreatment.TREATMENTID = treatmentModel.TreatmentId;
                    existingTreatment.TREATMENT1 = treatmentModel.Name;
                    existingTreatment.DESCRIPTION = treatmentModel.Description;

                    // on the database side feasibilties is an array, on the UI side it can be and is
                    // treated as a single record consiting of a criteria. So the DB -> UI sie gets
                    // the array and concatenates it. the UI insert or update wipes out all but one
                    // element of the array
                    if (treatmentModel.Feasilbility != null)
                    {
                        if (existingTreatment.FEASIBILITY == null)
                        {
                            existingTreatment.FEASIBILITY = new List<FEASIBILITY>();
                            existingTreatment.FEASIBILITY.Add(new FEASIBILITY());

                            FEASIBILITY feasibility = existingTreatment.FEASIBILITY.FirstOrDefault();

                            feasibility.TREATMENTID = treatmentModel.TreatmentId;
                            feasibility.FEASIBILITYID = 0;
                            feasibility.CRITERIA = treatmentModel.Feasilbility.Criteria;

                            existingTreatment.BEFOREANY = treatmentModel.Feasilbility.BeforeAny;
                            existingTreatment.BEFORESAME = treatmentModel.Feasilbility.BeforeSame;
                        }
                        else
                        {
                            FEASIBILITY feasibility = existingTreatment.FEASIBILITY.FirstOrDefault();

                            feasibility.TREATMENTID = treatmentModel.TreatmentId;
                            feasibility.CRITERIA = treatmentModel.Feasilbility.Criteria;
                            existingTreatment.BEFOREANY = treatmentModel.Feasilbility.BeforeAny;
                            existingTreatment.BEFORESAME = treatmentModel.Feasilbility.BeforeSame;
                        }
                    }

                    UpsertConsequences(treatmentModel, existingTreatment, db);

                    UpsertCost(treatmentModel, existingTreatment, db);

                    if (treatmentModel.Budgets.Count() > 0)
                    {
                        existingTreatment.BUDGET = treatmentModel.GetBudgets();
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

        public void UpsertCost(TreatmentScenarioModel treatmentScenarioModel,
            TREATMENT existingTreatment, BridgeCareContext db)
        {
            int dataIndex = 0;

            foreach (COST cost in existingTreatment.COST.ToList())
            {
                if (treatmentScenarioModel.Cost.Count() > dataIndex)
                {
                    cost.COST_ = treatmentScenarioModel.Cost[dataIndex].Cost;
                    cost.CRITERIA = treatmentScenarioModel.Cost[dataIndex].Criteria;
                    cost.ISFUNCTION = treatmentScenarioModel.Cost[dataIndex].IsFunction;
                    cost.UNIT = treatmentScenarioModel.Cost[dataIndex].Unit;
                }
                else
                {
                    db.Entry(cost).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the number of updated records exceeds the number of existing records
            while (treatmentScenarioModel.Cost.Count() > dataIndex)
            {
                var costEntity = new COST()
                {
                    COSTID = 0,
                    COST_ = treatmentScenarioModel.Cost[dataIndex].Cost,
                    CRITERIA = treatmentScenarioModel.Cost[dataIndex].Criteria,
                    ISFUNCTION = treatmentScenarioModel.Cost[dataIndex].IsFunction,
                    UNIT = treatmentScenarioModel.Cost[dataIndex].Unit,
                    TREATMENTID = treatmentScenarioModel.TreatmentId
                };
                existingTreatment.COST.Add(costEntity);
                dataIndex++;
            }
        }

        public void UpsertConsequences(TreatmentScenarioModel treatmentScenarioModel,
            TREATMENT existingTreatment, BridgeCareContext db)
        {
            int dataIndex = 0;

            foreach (CONSEQUENCE consequence in existingTreatment.CONSEQUENCES.ToList())
            {
                if (treatmentScenarioModel.Consequences.Count() > dataIndex)
                {
                    consequence.EQUATION = treatmentScenarioModel.Consequences[dataIndex].Equation;
                    consequence.CRITERIA = treatmentScenarioModel.Consequences[dataIndex].Criteria;
                    consequence.ATTRIBUTE_ = treatmentScenarioModel.Consequences[dataIndex].Attribute_;
                    consequence.CHANGE_ = treatmentScenarioModel.Consequences[dataIndex].Change;
                    consequence.ISFUNCTION = treatmentScenarioModel.Consequences[dataIndex].IsFunction;
                }
                else
                {
                    db.Entry(consequence).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the updated records exceed existing records
            while (treatmentScenarioModel.Consequences.Count() > dataIndex)
            {
                var consequenceEntity = new CONSEQUENCE()
                {
                    TREATMENTID = treatmentScenarioModel.TreatmentId,
                    EQUATION = treatmentScenarioModel.Consequences[dataIndex].Equation,
                    CRITERIA = treatmentScenarioModel.Consequences[dataIndex].Criteria,
                    ATTRIBUTE_ = treatmentScenarioModel.Consequences[dataIndex].Attribute_,
                    CHANGE_ = treatmentScenarioModel.Consequences[dataIndex].Change,
                    ISFUNCTION = treatmentScenarioModel.Consequences[dataIndex].IsFunction,
                    CONSEQUENCEID = 0
                };
                existingTreatment.CONSEQUENCES.Add(consequenceEntity);
                dataIndex++;
            }
        }
    }
}