using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.ApplicationLog;

namespace BridgeCare.Controllers
{
    public class ValidationController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IValidation validate;

        public ValidationController(IValidation validateMethods, BridgeCareContext context)
        {
            validate = validateMethods ?? throw new ArgumentNullException(nameof(validateMethods));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [ModelValidation("Function call not valid")]
        [Route("api/ValidateEquation")]
        [HttpPost]
        public HttpResponseMessage ValidateEquation(ValidateEquationModel data)
        {
            try
            {
                validate.ValidateEquation(data, db);
                return Request.CreateResponse(HttpStatusCode.OK, "OK");
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [ModelValidation("Function call not valid")]
        [Route("api/ValidateCriteria")]
        [HttpPost]
        public HttpResponseMessage ValidateCriteria([FromBody]ValidateCriteriaModel data)
        {
            try
            {
                string numberHits = validate.ValidateCriteria(data.Criteria, db);
                return Request.CreateResponse(HttpStatusCode.OK, numberHits);
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}