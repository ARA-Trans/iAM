using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class RemainingLifeLimitController : ApiController
    {
        private readonly IRemainingLifeLimit remainingLifeLimitDAL;
        private readonly BridgeCareContext db;

        public RemainingLifeLimitController(IRemainingLifeLimit remainingLifeLimitInterface, BridgeCareContext context)
        {
            remainingLifeLimitDAL = remainingLifeLimitInterface ??
                                 throw new ArgumentNullException(nameof(remainingLifeLimitInterface));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("api/GetScenarioRemainingLifeLimitLibrary/{selectedScenarioId}")]
        [ModelValidation("Given call is not valid")]
        [HttpGet]
        public RemainingLifeLimitLibraryModel Get(int selectedScenarioId) => remainingLifeLimitDAL.GetScenarioRemainingLifeLimitLibrary(selectedScenarioId, db);

        [Route("api/SaveScenarioRemainingLifeLimitLibrary")]
        [HttpPost]
        public RemainingLifeLimitLibraryModel Post([FromBody]RemainingLifeLimitLibraryModel data) => remainingLifeLimitDAL.SaveScenarioRemainingLifeLimitLibrary(data, db);
    }
}