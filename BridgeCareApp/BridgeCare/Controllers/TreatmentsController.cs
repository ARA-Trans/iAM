using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class TreatmentsController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ITreatments treatment;

        public TreatmentsController(ITreatments treatmentInterface, BridgeCareContext context)
        {
            treatment = treatmentInterface ?? throw new ArgumentNullException(nameof(treatmentInterface));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }
    
        /// <summary>
        /// Get: api/treatments
        /// </summary>
        [ModelValidation("Given simulation data is not valid")]
        [HttpGet]
        public IQueryable<TreatmentScenarioModel> Get(SimulationModel data) => treatment.GetTreatment(data, db);
    }
}