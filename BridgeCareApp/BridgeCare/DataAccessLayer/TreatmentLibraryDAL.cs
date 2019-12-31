using System.Collections.Generic;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System;

namespace BridgeCare.DataAccessLayer
{
    public class TreatmentLibraryDAL : ITreatmentLibrary
    {
        /// <summary>
        /// Fetches a simulation's treatment library data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private TreatmentLibraryModel GetSimulationTreatmentLibrary(int id, BridgeCareContext db)
        {
            var simulation = db.Simulations.Include(s => s.TREATMENTS)
                .Include(s => s.TREATMENTS.Select(t => t.FEASIBILITIES))
                .Include(s => s.TREATMENTS.Select(t => t.COSTS))
                .Include(s => s.TREATMENTS.Select(t => t.CONSEQUENCES))
                .Single(s => s.SIMULATIONID == id);

            return new TreatmentLibraryModel(simulation);
        }

        /// <summary>
        /// Fetches a simulation's treatment library data if it belongs to the user
        /// Throws a RowNotInTableException if no such simulation is found
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <returns>TreatmentLibraryModel</returns>
        public TreatmentLibraryModel GetOwnedSimulationTreatmentLibrary(int id, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id && (s.USERNAME == username || s.USERNAME == null)))
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's treatments.");
            return GetSimulationTreatmentLibrary(id, db);
        }

        public TreatmentLibraryModel GetAnySimulationTreatmentLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            return GetSimulationTreatmentLibrary(id, db);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's treatment library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">TreatmentLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TreatmentLibraryModel</returns>
        private TreatmentLibraryModel SaveSimulationTreatmentLibrary(TreatmentLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            var simulation = db.Simulations.Include(s => s.TREATMENTS)
                .Include(s => s.TREATMENTS.Select(t => t.FEASIBILITIES))
                .Include(s => s.TREATMENTS.Select(t => t.COSTS))
                .Include(s => s.TREATMENTS.Select(t => t.CONSEQUENCES))
                .Single(s => s.SIMULATIONID == id);

            if (simulation.TREATMENTS.Any())
            {
                simulation.TREATMENTS.ToList().ForEach(treatmentEntity =>
                {
                    var treatmentModel = model.Treatments
                        .SingleOrDefault(t => t.Id == treatmentEntity.TREATMENTID.ToString());

                    if (treatmentModel == null)
                        TreatmentsEntity.DeleteEntry(treatmentEntity, db);
                    else
                    {
                        treatmentModel.matched = true;
                        treatmentModel.UpdateTreatment(treatmentEntity);

                        if (treatmentEntity.FEASIBILITIES.Any())
                        {
                            var feasibilityModel = treatmentModel.Feasibility;

                            var feasibilitiesToDelete = treatmentEntity.FEASIBILITIES
                                .Where(f => f.FEASIBILITYID.ToString() != feasibilityModel.Id).ToList();
                            feasibilitiesToDelete.ForEach(feasibilityToDelete => FeasibilityEntity.DeleteEntry(feasibilityToDelete, db));

                            if (treatmentEntity.FEASIBILITIES.Any(
                                f => f.FEASIBILITYID.ToString() == feasibilityModel.Id))
                            {
                                feasibilityModel.matched = true;
                                var feasibilityEntity = treatmentEntity.FEASIBILITIES
                                    .Single(f => f.FEASIBILITYID.ToString() == feasibilityModel.Id);
                                feasibilityEntity.CRITERIA = feasibilityModel.Criteria;
                            }
                        }

                        if (!treatmentModel.Feasibility.matched)
                            treatmentEntity.FEASIBILITIES
                                .Add(new FeasibilityEntity(treatmentEntity.TREATMENTID, treatmentModel.Feasibility));

                        if (treatmentEntity.COSTS.Any())
                        {
                            treatmentEntity.COSTS.ToList().ForEach(costEntity =>
                            {
                                var costModel = treatmentModel.Costs
                                    .SingleOrDefault(c => c.Id == costEntity.COSTID.ToString());

                                if (costModel == null)
                                    CostsEntity.DeleteEntry(costEntity, db);
                                else
                                {
                                    costModel.matched = true;
                                    costModel.UpdateCost(costEntity);
                                }
                            });
                        }

                        if (treatmentModel.Costs.Any(m => !m.matched))
                        {
                            treatmentModel.Costs
                                .Where(costModel => !costModel.matched)
                                .Select(costModel => new CostsEntity(treatmentEntity.TREATMENTID, costModel))
                                .ToList().ForEach(costsEntity => treatmentEntity.COSTS.Add(costsEntity));
                        }

                        if (treatmentEntity.CONSEQUENCES.Any())
                        {
                            treatmentEntity.CONSEQUENCES.ToList().ForEach(consequenceEntity =>
                            {
                                var consequenceModel = treatmentModel.Consequences
                                    .SingleOrDefault(c => c.Id == consequenceEntity.CONSEQUENCEID.ToString());

                                if (consequenceModel == null)
                                    ConsequencesEntity.DeleteEntry(consequenceEntity, db);
                                else
                                {
                                    consequenceModel.matched = true;
                                    consequenceModel.UpdateConsequence(consequenceEntity);
                                }
                            });
                        }

                        if (treatmentModel.Consequences.Any(m => !m.matched))
                        {
                            treatmentModel.Consequences
                                .Where(consequenceModel => !consequenceModel.matched)
                                .Select(consequenceModel => new ConsequencesEntity(treatmentEntity.TREATMENTID, consequenceModel))
                                .ToList().ForEach(consequencesEntity => treatmentEntity.CONSEQUENCES.Add(consequencesEntity));
                        }
                    }
                });
            }

            if (model.Treatments.Any(m => !m.matched))
            {
                model.Treatments
                    .Where(treatmentModel => !treatmentModel.matched)
                    .Select(treatmentModel =>
                    {
                        var treatment = new TreatmentsEntity(id, treatmentModel)
                        {
                            FEASIBILITIES = new List<FeasibilityEntity>()
                            {
                                new FeasibilityEntity(treatmentModel.Feasibility)
                            }
                        };
                        if (treatmentModel.Costs.Count > 0)
                            treatment.COSTS = treatmentModel.Costs.Select(c => new CostsEntity(c)).ToList();
                        if (treatmentModel.Consequences.Count > 0)
                            treatment.CONSEQUENCES =
                                treatmentModel.Consequences.Select(c => new ConsequencesEntity(c)).ToList();
                        return treatment;
                    })
                    .ToList().ForEach(treatmentEntity => simulation.TREATMENTS.Add(treatmentEntity));
            }

            db.SaveChanges();

            return new TreatmentLibraryModel(simulation);
        }

        public TreatmentLibraryModel SaveOwnedSimulationTreatmentLibrary(TreatmentLibraryModel model, BridgeCareContext db, string username)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id && s.USERNAME == username))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's treatments.");
            return SaveSimulationTreatmentLibrary(model, db);
        }

        public TreatmentLibraryModel SaveAnySimulationTreatmentLibrary(TreatmentLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");
            return SaveSimulationTreatmentLibrary(model, db);
        }
    }
}