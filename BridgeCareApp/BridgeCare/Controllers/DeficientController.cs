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
        [Route("api/GetDeficients")]
        [ModelValidation("Given Scenario Id is not valid")]
        public List<DeficientModel> GetDeficients(int selectedScenarioId)
        {
            return deficientRepo.GetDeficients(selectedScenarioId, db);
        }

        [HttpPost]
        [Route("api/SaveDeficients")]
        [ModelValidation("Given deficients are not valid")]
        public List<DeficientModel> SaveDeficients(int selectedScenarioId, [FromBody] List<DeficientModel> data)
        {
            return deficientRepo.SaveDeficients(selectedScenarioId, data, db);
        }
    }
}