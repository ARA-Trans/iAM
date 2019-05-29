using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class TreatmentLibraryController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ITreatmentLibrary treatmentLibrary;

        public TreatmentLibraryController(ITreatmentLibrary treatmentInterface, BridgeCareContext context)
        {
            treatmentLibrary = treatmentInterface ?? throw new ArgumentNullException(nameof(treatmentInterface));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("api/GetScenarioTreatmentLibrary/{selectedScenarioId}")]
        [ModelValidation("Given call is not valid")]
        [HttpGet]
        public TreatmentLibraryModel Get(int selectedScenarioId) => treatmentLibrary.GetScenarioTreatmentLibrary(selectedScenarioId, db);

        [Route("api/SaveScenarioTreatmentLibrary")]
        [HttpPost]
        public TreatmentLibraryModel Post([FromBody]TreatmentLibraryModel data) => treatmentLibrary.SaveScenarioTreatmentLibrary(data, db);
    }
}