using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class DetailedReportController : ApiController
    {
        private readonly IReportCreator reportCreator;

        public DetailedReportController(IReportCreator reportCreator)
        {
            this.reportCreator = reportCreator ?? throw new ArgumentNullException(nameof(reportCreator));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's detailed report data
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/GetDetailedReport")]
        [ModelValidation("The scenario and/or network id are invalid.")]
        [Filters.RestrictAccess]
        public HttpResponseMessage GetDetailedReport([FromBody] SimulationModel model)
        {
            var response = Request.CreateResponse();
            response.Content = new ByteArrayContent(reportCreator.CreateExcelReport(model));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "DetailedReport.xlsx"
            };

            return response;
            //return Ok(response);
        }
    }
}