using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using BridgeCare.ExceptionHandling;

namespace BridgeCare
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            System.Web.Http.GlobalConfiguration.Configuration.Filters.Add(
                new UnhandledExceptionFilterAttribute()
                    .Register<RowNotInTableException>((exception, request) =>
                        request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message)
                    )
                    .Register<ConstraintException>((exception, request) =>
                        request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message)
                    )
                    .Register<OutOfMemoryException>((exception, request) =>
                        request.CreateErrorResponse(
                            HttpStatusCode.ServiceUnavailable, 
                            $"Services are temporarily unavailable::{exception.Message}"
                        )
                    )
                    .Register<Exception>((exception, request) =>
                        request.CreateErrorResponse(
                            HttpStatusCode.InternalServerError, 
                            $"Server error::{exception.Message}"
                        )
                    )
                    .Register<SqlException>((exception, request) =>
                        {
                            var sqlException = exception as SqlException;

                            if (sqlException?.Number > 50000)
                            {
                                return request.CreateErrorResponse(
                                    HttpStatusCode.BadRequest,
                                    sqlException.Message.Replace(Environment.NewLine, string.Empty)
                                );
                            }

                            if (sqlException?.Number == -2 || sqlException?.Number == 11)
                            {
                                return request.CreateErrorResponse(
                                    HttpStatusCode.RequestTimeout,
                                    "The server timed out. Please try again later."
                                );
                            }

                            return request.CreateErrorResponse(
                                HttpStatusCode.InternalServerError,
                                $"Server error::{exception.Message}"
                            );
                        }
                    )
                    .Register<UnauthorizedAccessException>((exception, request) =>
                        {
                            return request.CreateErrorResponse(HttpStatusCode.Unauthorized, exception.Message);
                        }
                    )
            );
        }
    }
}
