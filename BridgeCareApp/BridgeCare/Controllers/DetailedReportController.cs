using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Net;
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

        // POST: api/DetailedReport
        [HttpPost]
        [ModelValidation("Given Network Id and/or Simulation Id are not valid")]
        public IHttpActionResult CreateDetailedReport([FromBody] SimulationModel data)
        {
            var response = Request.CreateResponse();
            response.Content = new ByteArrayContent(reportCreator.CreateExcelReport(data));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "DetailedReport.xlsx"
            };
            return Ok(response);
        }
    }
}