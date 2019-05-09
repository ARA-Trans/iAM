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

        [Route("api/GetScenarioTreatmentLibrary/{treatmentId}")]
        [ModelValidation("Given call is not valid")]
        [HttpGet]
        public TreatmentScenarioModel GetTreatment(int treatmentId) => treatment.GetTreatment(treatmentId, db);

        [Route("api/GetScenarioTreatmentLibraries/{simulationId}")]
        [ModelValidation("Given call is not valid")]
        [HttpGet]
        public IQueryable<TreatmentScenarioModel> GetTreatments(int simulationId) => treatment.GetTreatments(simulationId, db);

        [Route("api/CreateScenarioTreatmentLibrary")]
        [HttpPost]
        public TreatmentScenarioModel CreateTreatment([FromBody]TreatmentScenarioModel data) => treatment.CreateTreatment(data, db);

        [Route("api/UpsertScenarioTreatmentLibrary")]
        [HttpPost]
        public TreatmentScenarioModel UpsertTreatment([FromBody]TreatmentScenarioModel data) => treatment.UpsertTreatment(data, db);

    }
}