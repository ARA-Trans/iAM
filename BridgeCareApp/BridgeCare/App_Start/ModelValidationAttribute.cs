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
                var testb = actionContext.ActionDescriptor;
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.BadRequest, "The argument cannot be null");
            }
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.BadRequest, Message);
            }
        }

        private readonly Func<Dictionary<string, object>, bool> _validate;
        private string Message { get; set; }

        public ModelValidationAttribute() : this(arguments => arguments.ContainsValue(null))
        { }
        public ModelValidationAttribute(string message) : this(arguments => arguments.ContainsValue(null))
        {
            Message = message;
        }

        public ModelValidationAttribute(Func<Dictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }
    }
}