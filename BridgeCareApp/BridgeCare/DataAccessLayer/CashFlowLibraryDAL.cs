using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
    public class CashFlowLibraryDAL : ICashFlowLibrary
    {
        /// <summary>
        /// Fetches a simulation's cash flow library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns></returns>
        public CashFlowLibraryModel GetSimulationCashFlowLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");

            var simulation = db.Simulations
                .Include(s => s.SPLIT_TREATMENTS)
                .Include(s => s.SPLIT_TREATMENTS.Select(st => st.SPLIT_TREATMENT_LIMITS))
                .Single(s => s.SIMULATIONID == id);

            return new CashFlowLibraryModel(simulation);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's cash flow library data
        /// Throws a RowNotInTableException if no simulation is found
        /// </summary>
        /// <param name="model">CashFlowLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns></returns>
        public CashFlowLibraryModel SaveSimulationCashFlowLibrary(CashFlowLibraryModel model, BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario found with id {id}");

            var simulation = db.Simulations
                .Include(s => s.SPLIT_TREATMENTS)
                .Include(s => s.SPLIT_TREATMENTS.Select(st => st.SPLIT_TREATMENT_LIMITS))
                .Single(s => s.SIMULATIONID == id);

            if (simulation.SPLIT_TREATMENTS.Any())
            {
                simulation.SPLIT_TREATMENTS.ToList().ForEach(splitTreatment =>
                {
                    var splitTreatmentModel =
                        model.SplitTreatments.SingleOrDefault(m =>
                            m.Id == splitTreatment.SPLIT_TREATMENT_ID.ToString());

                    if (splitTreatmentModel == null)
                        SplitTreatmentEntity.DeleteEntry(splitTreatment, db);
                    else
                    {
                        splitTreatmentModel.matched = true;
                        splitTreatmentModel.UpdateSplitTreatment(splitTreatment);

                        if (splitTreatment.SPLIT_TREATMENT_LIMITS.Any())
                        {
                            splitTreatment.SPLIT_TREATMENT_LIMITS.ToList().ForEach(splitTreatmentLimit =>
                            {
                                var splitTreatmentLimitModel =
                                    splitTreatmentModel.SplitTreatmentLimits.SingleOrDefault(m =>
                                        m.Id == splitTreatmentLimit.SPLIT_TREATMENT_LIMIT_ID.ToString());

                                if (splitTreatmentLimitModel == null)
                                    SplitTreatmentLimitEntity.DeleteEntry(splitTreatmentLimit, db);
                                else
                                {
                                    splitTreatmentLimitModel.matched = true;
                                    splitTreatmentLimitModel.UpdateSplitTreatment(splitTreatmentLimit);
                                }
                            }); 
                        }

                        if (splitTreatmentModel.SplitTreatmentLimits.Any(m => !m.matched))
                            db.SplitTreatmentLimits.AddRange(splitTreatmentModel.SplitTreatmentLimits
                                .Where(splitTreatmentLimitModel => !splitTreatmentLimitModel.matched)
                                .Select(splitTreatmentLimitModel =>
                                    new SplitTreatmentLimitEntity(splitTreatment.SPLIT_TREATMENT_ID, splitTreatmentLimitModel))
                                .ToList()
                            );
                    }
                });
            }

            if (model.SplitTreatments.Any(m => !m.matched))
                db.SplitTreatments.AddRange(model.SplitTreatments
                    .Where(splitTreatmentModel => !splitTreatmentModel.matched)
                    .Select(splitTreatmentModel => new SplitTreatmentEntity(id, splitTreatmentModel))
                    .ToList()
                );

            db.SaveChanges();

            return new CashFlowLibraryModel(simulation);
        }
    }
}