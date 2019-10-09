using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;

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
        public TreatmentLibraryModel GetSimulationTreatmentLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");

            var simulation = db.Simulations.Include(s => s.TREATMENTS)
                .Include(s => s.TREATMENTS.Select(t => t.FEASIBILITIES))
                .Include(s => s.TREATMENTS.Select(t => t.COSTS))
                .Include(s => s.TREATMENTS.Select(t => t.CONSEQUENCES))
                .Single(s => s.SIMULATIONID == id);

            return new TreatmentLibraryModel(simulation);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's treatment library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">TreatmentLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>TreatmentLibraryModel</returns>
        public TreatmentLibraryModel SaveSimulationTreatmentLibrary(TreatmentLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");

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
                            treatmentEntity.FEASIBILITIES.ToList().ForEach(feasibilityEntity =>
                            {
                                var feasibilityModel = treatmentModel.Feasibility;

                                if (feasibilityModel.Id != feasibilityEntity.FEASIBILITYID.ToString())
                                    FeasibilityEntity.DeleteEntry(feasibilityEntity, db);
                                else
                                {
                                    feasibilityModel.matched = true;
                                    feasibilityEntity.CRITERIA = feasibilityModel.Criteria;
                                }
                            });
                        }

                        if (!treatmentModel.Feasibility.matched)
                            db.Feasibilities
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
                            db.Costs
                                .AddRange(treatmentModel.Costs
                                    .Where(costModel => !costModel.matched)
                                    .Select(costModel => new CostsEntity(treatmentEntity.TREATMENTID, costModel))
                                    .ToList()
                                );
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
                            db.Consequences
                                .AddRange(treatmentModel.Consequences
                                    .Where(consequenceModel => !consequenceModel.matched)
                                    .Select(consequenceModel => new ConsequencesEntity(treatmentEntity.TREATMENTID, consequenceModel))
                                    .ToList()
                                );
                        }
                    }
                });
            }

            if (model.Treatments.Any(m => !m.matched))
            {
                db.Treatments
                    .AddRange(model.Treatments
                        .Where(treatmentModel => !treatmentModel.matched)
                        .Select(treatmentModel => new TreatmentsEntity(id, treatmentModel))
                        .ToList()
                    );
            }

            db.SaveChanges();

            return new TreatmentLibraryModel(simulation);
        }
    }
}