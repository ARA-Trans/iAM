using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace System.Web.Http.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ModelValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_validate(actionContext.ActionArguments))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.BadRequest, "The argument cannot be null");
            }
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }

        private readonly Func<Dictionary<string, object>, bool> _validate;

        public ModelValidationAttribute() : this(arguments => arguments.ContainsValue(null))
        { }

        public ModelValidationAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }
    }
}