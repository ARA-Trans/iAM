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
        private readonly BridgeCareContext db;
        private readonly IDeficient deficientRepo;

        public DeficientController() { }

        public DeficientController(IDeficient deficientRepository, BridgeCareContext context)
        {
            deficientRepo = deficientRepository ?? throw new ArgumentNullException(nameof(deficientRepository));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("api/GetScenarioDeficientLibrary/{selectedScenarioId}")]
        [ModelValidation("Given Scenario Id is not valid")]
        public DeficientLibraryModel GetScenarioDeficientLibrary(int selectedScenarioId)
        {
            return deficientRepo.GetScenarioDeficientLibrary(selectedScenarioId, db);
        }

        [HttpPost]
        [Route("api/SaveScenarioDeficientLibrary")]
        [ModelValidation("Given deficients are not valid")]
        public DeficientLibraryModel SaveScenarioDeficientLibrary([FromBody] DeficientLibraryModel data)
        {
            return deficientRepo.SaveScenarioDeficientLibrary(data, db);
        }
    }
}