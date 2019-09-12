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
        [Route("api/GetScenarioPriorityLibrary/{selectedScenarioId}")]
        [ModelValidation("Given Scenario Id is not valid")]
        public PriorityLibraryModel GetScenarioPriorityLibrary(int selectedScenarioId)
        {
            return priorityRepo.GetScenarioPriorityLibrary(selectedScenarioId, db);
        }

        [HttpPost]
        [Route("api/SaveScenarioPriorityLibrary")]
        [ModelValidation("Given priorities are not valid")]
        public PriorityLibraryModel SaveScenarioPriorityLibrary([FromBody]PriorityLibraryModel data)
        {
            return priorityRepo.SaveScenarioPriorityLibrary(data, db);
        }
    }
}