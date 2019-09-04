using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class TargetController: ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ITarget targetRepo;

        public TargetController() { }

        public TargetController(ITarget targetRepository, BridgeCareContext context)
        {
            targetRepo = targetRepository ?? throw new ArgumentNullException(nameof(targetRepository));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("api/GetScenarioTargetLibrary/{selectedScenarioId}")]
        [ModelValidation("Given Scenario Id is not valid")]
        public TargetLibraryModel GetScenarioTargetLibrary(int selectedScenarioId)
        {
            return targetRepo.GetScenarioTargetLibrary(selectedScenarioId, db);
        }

        [HttpPost]
        [Route("api/SaveScenarioTargetLibrary")]
        [ModelValidation("Given targets are not valid")]
        public TargetLibraryModel SaveScenarioTargetLibrary([FromBody]TargetLibraryModel data)
        {
            return targetRepo.SaveScenarioTargetLibrary(data, db);
        }
    }
}