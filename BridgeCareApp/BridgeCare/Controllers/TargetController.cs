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
        [Route("api/GetTargets")]
        [ModelValidation("Given Scenario Id is not valid")]
        public List<TargetModel> GetTargets(int selectedScenarioId)
        {
            return targetRepo.GetTargets(selectedScenarioId, db);
        }

        [HttpPost]
        [Route("api/SaveTargets")]
        [ModelValidation("Given targets are not valid")]
        public List<TargetModel> SaveTargets(int selectedScenarioId, [FromBody]List<TargetModel> data)
        {
            return targetRepo.SaveTargets(selectedScenarioId, data, db);
        }
    }
}