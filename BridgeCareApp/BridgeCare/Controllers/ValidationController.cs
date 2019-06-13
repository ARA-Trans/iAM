using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

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
        public bool ValidateEquation(ValidateModel data)
        {
            if (data.isFunction)
            {
                return validate.ValidateEquation(data, db);
            }
            else
            {
                return validate.ValidateCriteria(data, db);
            }

           }
}