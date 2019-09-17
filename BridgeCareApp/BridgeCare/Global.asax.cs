using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using BridgeCare.ApplicationLog;
using BridgeCare.Models;

namespace BridgeCare
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Filters.Add(
                new UnhandledExceptionFilterAttribute()
                    .Register<RowNotInTableException>((exception, request) => {
                            var response = request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
                            return response;
                        })
                    .Register<ConstraintException>(HttpStatusCode.BadRequest)
                    .Register<OutOfMemoryException>(HttpStatusCode.ServiceUnavailable)
                    .Register<Exception>(HttpStatusCode.InternalServerError)
                    .Register<SqlException>(
                        (exception, request) =>
                        {
                            var sqlException = exception as SqlException;
                            if (sqlException.Number > 50000)
                            {
                                var response = request.CreateResponse(HttpStatusCode.BadRequest);
                                response.ReasonPhrase = sqlException.Message.Replace(Environment.NewLine, String.Empty);
                                response.Content = new StringContent(response.ReasonPhrase);

                                return response;
                            }

                            if (sqlException.Number == -2 || sqlException.Number == 11)
                            {
                                var response = request.CreateResponse(HttpStatusCode.RequestTimeout);
                                response.ReasonPhrase = "The server timed out. Please try again later.";
                                return response;
                            }

                            return request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                    )
            );
        }
    }
}
