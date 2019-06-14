﻿using BridgeCare.ApplicationLog;
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
    public class TreatmentLibrary : ITreatmentLibrary
    {
        public TreatmentLibrary()
        {
        }

        public TreatmentLibraryModel GetScenarioTreatmentLibrary(int selectedScenarioId, BridgeCareContext db)
        {
            /// Query will fetch all data with data.Simulationid and fill each TreatmentScenarioModel
            /// object from four tables based on a common TreatmentId
            try
            {
                var treatmentLibraryModel = db.SIMULATIONS
                    .Include("TREATMENTS").Include("CONSEQUENCES").Include("FEASIBILITY").Include("COST")
                    .Where(b => b.SIMULATIONID == selectedScenarioId)
                    .Select(b => new TreatmentLibraryModel()
                    {
                        SimulationId = b.SIMULATIONID,
                        Description = b.COMMENTS,
                        Name = b.SIMULATION1,
                        Treatments = b.TREATMENTS.Select(p => new TreatmentModel
                        {
                            TreatmentId = p.TREATMENTID.ToString(),
                            Name = p.TREATMENT1,
                            Budget = p.BUDGET,
                            OMS_IS_EXCLUSIVE = p.OMS_IS_EXCLUSIVE,
                            OMS_IS_REPEAT = p.OMS_IS_REPEAT,
                            OMS_REPEAT_START = p.OMS_REPEAT_START,
                            OMS_REPEAT_INTERVAL = p.OMS_REPEAT_INTERVAL,
                            Costs = p.COST.Select(q => new CostModel
                            {
                                Cost = q.COST_,
                                CostId = q.COSTID.ToString(),
                                Criteria = q.CRITERIA,
                                IsFunction = q.ISFUNCTION,
                                Unit = q.UNIT
                            }).ToList(),
                            Feasibilities = p.FEASIBILITY.Select(m => new FeasibilityModel
                            {
                                Criteria = m.CRITERIA,
                                FeasibilityId = m.FEASIBILITYID.ToString(),
                                BeforeAny = p.BEFOREANY,
                                BeforeSame = p.BEFORESAME
                            }).ToList(),
                            Consequences = p.CONSEQUENCES.Select(n => new ConsequenceModel
                            {
                                ConsequenceId = n.CONSEQUENCEID.ToString(),
                                Criteria = n.CRITERIA,
                                Attribute_ = n.ATTRIBUTE_,
                                Change = n.CHANGE_,
                                IsFunction = n.ISFUNCTION,
                                Equation = n.EQUATION,
                            }).ToList()
                        }).ToList()
                    }).SingleOrDefault();

                foreach (var treatment in treatmentLibraryModel.Treatments)
                {
                    treatment.SetBudgets();
                    if (!treatment.Feasibilities.Any())
                    {
                        treatment.Feasilbility = null;
                        continue;
                    }

                    treatment.Feasilbility = new FeasibilityModel();

                    foreach (var feasibility in treatment.Feasibilities)
                    {
                        treatment.Feasilbility.Aggregate(feasibility);
                        treatment.Feasilbility.BeforeAny = treatment.BeforeAny;
                        treatment.Feasilbility.BeforeSame = treatment.BeforeSame;
                    }
                }
                return treatmentLibraryModel;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Get Treatment Library Failed");
            }
            return new TreatmentLibraryModel();
        }

        public TreatmentLibraryModel SaveScenarioTreatmentLibrary(TreatmentLibraryModel requestedModel, BridgeCareContext db)
        {
            try
            {
                var existingSimulation = db.SIMULATIONS.FirstOrDefault(p => p.SIMULATIONID == requestedModel.SimulationId);
                if (existingSimulation != null) existingSimulation.SIMULATION1 = requestedModel.Name;

                var existingTreatments = db.Treatments
                    .Include("CONSEQUENCES").Include("FEASIBILITY").Include("COST")
                    .Where(p => p.SIMULATIONID == requestedModel.SimulationId).ToList();

                // the treatment tables get deleted index by index and when they do all the records in
                // sub tables also get deleted, the records in the sub tables are tied to one treatment.
                // if a cost or consequence record is deleted all the records for the particular treatment 
                // shift up and occupy the lower Id's
                foreach (var existingTreatment in existingTreatments.ToList())
                {
                    if (requestedModel.Treatments.Any(t => t.TreatmentId == existingTreatment.TREATMENTID.ToString()))
                    {
                        var treatmentModel =
                          requestedModel.Treatments.SingleOrDefault(t => t.TreatmentId == existingTreatment.TREATMENTID.ToString());
                        if (treatmentModel == null) continue;

                        treatmentModel.matched = true;
                        existingTreatment.SIMULATIONID = requestedModel.SimulationId;
                        existingTreatment.TREATMENT1 = treatmentModel.Name;
                        existingTreatment.DESCRIPTION = treatmentModel.Name;
                        existingTreatment.BUDGET = treatmentModel.Budgets.Any() ? treatmentModel.GetBudgets() : null;

                        // on the database side feasibilties is an array, on the UI side it can be and is
                        // treated as a single record consisting of a criteria. So the DB -> UI side gets
                        // the array and concatenates it. the UI insert or update wipes out all but one
                        // element of the array
                        if (treatmentModel.Feasilbility != null)
                        {
                            if (existingTreatment.FEASIBILITY.Any())
                            {
                                FEASIBILITY feasibility = existingTreatment.FEASIBILITY.FirstOrDefault();
                                if (feasibility != null) feasibility.CRITERIA = treatmentModel.Feasilbility.Criteria;
                                existingTreatment.BEFOREANY = treatmentModel.Feasilbility.BeforeAny;
                                existingTreatment.BEFORESAME = treatmentModel.Feasilbility.BeforeSame;
                            }
                            else
                            {
                                FEASIBILITY feasibility = new FEASIBILITY
                                {
                                    TREATMENTID = existingTreatment.TREATMENTID,
                                    CRITERIA = treatmentModel.Feasilbility.Criteria
                                };
                                existingTreatment.BEFOREANY = treatmentModel.Feasilbility.BeforeAny;
                                existingTreatment.BEFORESAME = treatmentModel.Feasilbility.BeforeSame;
                                existingTreatment.FEASIBILITY.Add(feasibility);
                            }
                        }

                        UpsertConsequences(treatmentModel, existingTreatment, db);

                        UpsertCost(treatmentModel, existingTreatment, db);
                    }
                    else
                    {
                        TREATMENT.delete(existingTreatment, db);
                    }
                }

                db.SaveChanges();

                if (requestedModel.Treatments.Any(t => !t.matched))
                {
                    var newTreatmentsData = requestedModel.Treatments.Where(t => !t.matched).ToList();
                    foreach (var treatment in newTreatmentsData)
                    {
                        db.Treatments.Add(
                            CreateTreatment(treatment, existingSimulation.SIMULATIONID)
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Update/Insert Treatment Library Failed");
            }

            return GetScenarioTreatmentLibrary(requestedModel.SimulationId, db);
        }

        public void UpsertCost(TreatmentModel treatmentScenarioModel,
            TREATMENT existingTreatment, BridgeCareContext db)
        {
            int dataIndex = 0;

            foreach (COST cost in existingTreatment.COST.ToList())
            {
                if (treatmentScenarioModel.Costs.Count() > dataIndex)
                {
                    cost.COST_ = treatmentScenarioModel.Costs[dataIndex].Cost;
                    cost.CRITERIA = treatmentScenarioModel.Costs[dataIndex].Criteria;
                    cost.ISFUNCTION = treatmentScenarioModel.Costs[dataIndex].IsFunction;
                }
                else
                {
                    db.Entry(cost).State = EntityState.Deleted;
                }
                dataIndex++;
            }
            //these must be inserts as the number of updated records exceeds the number of existing records
            while (treatmentScenarioModel.Costs.Count() > dataIndex)
            {
                var costEntity = new COST()
                {
                    COST_ = treatmentScenarioModel.Costs[dataIndex].Cost,
                    CRITERIA = treatmentScenarioModel.Costs[dataIndex].Criteria,
                    ISFUNCTION = treatmentScenarioModel.Costs[dataIndex].IsFunction,
                    TREATMENTID = existingTreatment.TREATMENTID
                };
                existingTreatment.COST.Add(costEntity);
                dataIndex++;
            }
        }

        public void UpsertConsequences(TreatmentModel treatmentScenarioModel,
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
                    TREATMENTID = existingTreatment.TREATMENTID,
                    EQUATION = treatmentScenarioModel.Consequences[dataIndex].Equation,
                    CRITERIA = treatmentScenarioModel.Consequences[dataIndex].Criteria,
                    ATTRIBUTE_ = treatmentScenarioModel.Consequences[dataIndex].Attribute_,
                    CHANGE_ = treatmentScenarioModel.Consequences[dataIndex].Change,
                    ISFUNCTION = treatmentScenarioModel.Consequences[dataIndex].IsFunction,
                };
                existingTreatment.CONSEQUENCES.Add(consequenceEntity);
                dataIndex++;
            }
        }

        public TREATMENT CreateTreatment(TreatmentModel data, int simulationId)
        {
            var newTreatment = new TREATMENT
            {
                SIMULATIONID = simulationId,
                TREATMENT1 = data.Name,
                BEFOREANY = 0,
                BEFORESAME = 0,
                DESCRIPTION = data.Name
            };

            if (data.Feasilbility != null)
            {
                if (data.Feasilbility.BeforeAny > 0) newTreatment.BEFOREANY = data.Feasilbility.BeforeAny;
                if (data.Feasilbility.BeforeSame > 0) newTreatment.BEFORESAME = data.Feasilbility.BeforeSame;
                newTreatment.FEASIBILITY = new List<FEASIBILITY>()
                {
                    new FEASIBILITY()
                    {
                        CRITERIA = data.Feasilbility.Criteria ?? ""
                    }
                };
            }

            if (data.Costs.Any())
            {
                newTreatment.COST = new List<COST>();
                foreach (var cost in data.Costs)
                {
                    newTreatment.COST.Add(new COST()
                    {
                        COST_ = cost.Cost,
                        CRITERIA = cost.Criteria,
                        ISFUNCTION = cost.IsFunction
                    });
                }
            }

            if (data.Consequences.Any())
            {
                newTreatment.CONSEQUENCES = new List<CONSEQUENCE>();
                foreach (var consequence in data.Consequences)
                {
                    newTreatment.CONSEQUENCES.Add(new CONSEQUENCE()
                    {
                        EQUATION = consequence.Equation,
                        CRITERIA = consequence.Criteria,
                        ATTRIBUTE_ = consequence.Attribute_,
                        CHANGE_ = consequence.Change,
                        ISFUNCTION = consequence.IsFunction
                    });
                }
            }

            if (data.Budgets.Any())
            {
                newTreatment.BUDGET = data.GetBudgets();
            }

            return newTreatment;
        }
    }
}