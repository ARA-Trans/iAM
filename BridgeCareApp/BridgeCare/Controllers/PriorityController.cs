using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class PriorityController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IPriority priorityRepo;

        public PriorityController() { }

        public PriorityController(IPriority priorityRepository, BridgeCareContext context)
        {
            priorityRepo = priorityRepository ?? throw new ArgumentNullException(nameof(priorityRepository));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("api/GetPriorities")]
        [ModelValidation("Given Scenario Id is not valid")]
        public List<PriorityModel> GetPriorities(int selectedScenarioId)
        {
            return priorityRepo.GetPriorities(selectedScenarioId, db);
        }

        [HttpPost]
        [Route("api/SavePriorities")]
        [ModelValidation("Given priorities are not valid")]
        public List<PriorityModel> SavePriorities(int selectedScenarioId, [FromBody]List<PriorityModel> data)
        {
            return priorityRepo.SavePriorities(selectedScenarioId, data, db);
        }
    }
}