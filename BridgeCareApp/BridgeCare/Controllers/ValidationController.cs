﻿using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class ValidationController : ApiController
    {
        private readonly IValidation repo;
        private readonly BridgeCareContext db;

        public ValidationController(IValidation repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for validating an equation
        /// </summary>
        /// <param name="model">ValidateEquationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/ValidateEquation")]
        [ModelValidation("The equation data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult ValidateEquation(ValidateEquationModel model)
        {
            return Ok(repo.ValidateEquation(model, db));
        }

        /// <summary>
        /// API endpoint for validating a criteria
        /// </summary>
        /// <param name="model">ValidateCriteriaModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/ValidateCriteria")]
        [ModelValidation("The criteria data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult ValidateCriteria([FromBody]ValidateCriteriaModel model) =>
            Ok(repo.ValidateCriteria(model.Criteria, db));
    }
}