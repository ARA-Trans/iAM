using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class DeficientController: ApiController
    {
        private readonly IDeficient repo;
        private readonly BridgeCareContext db;

        public DeficientController() { }

        public DeficientController(IDeficient repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        [Route("api/GetScenarioDeficientLibrary/{selectedScenarioId}")]
        [ModelValidation("The scenario id is not valid.")]
        public IHttpActionResult GetScenarioDeficientLibrary(int selectedScenarioId)
            => Ok(repo.GetScenarioDeficientLibrary(selectedScenarioId, db));

        [HttpPost]
        [Route("api/SaveScenarioDeficientLibrary")]
        [ModelValidation("The deficient data is not valid.")]
        public IHttpActionResult SaveScenarioDeficientLibrary([FromBody] DeficientLibraryModel data)
            => Ok(repo.SaveScenarioDeficientLibrary(data, db));
    }
}