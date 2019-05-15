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

        [Route("api/GetScenarioTreatmentLibrary/{simulationId}")]
        [ModelValidation("Given call is not valid")]
        [HttpGet]
        public TreatmentsModel GetTreatments(int simulationId) => treatment.GetTreatments(simulationId, db);

        [Route("api/UpsertScenarioTreatmentLibrary")]
        [HttpPost]
        public TreatmentsModel UpsertTreatment([FromBody]TreatmentsModel data) => treatment.UpsertTreatment(data, db);

    }
}