using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.ApplicationLog;

namespace BridgeCare.Controllers
{
    public class SummaryReportController : ApiController
    {
        private readonly ISummaryReportGenerator summaryReportGenerator;
        private readonly IBridgeData bridgeData;
        private readonly BridgeCareContext db;

        public SummaryReportController(ISummaryReportGenerator summaryReportGenerator, IBridgeData bridgeDataInterface, BridgeCareContext context)
        {
            this.summaryReportGenerator = summaryReportGenerator;
            this.bridgeData = bridgeDataInterface;
            this.db = context;
        }

        [HttpGet]
        [Route("api/GetSummaryReportMissingAttributes")]
        public IHttpActionResult GetSummaryReportMissingAttributes(int simulationId, int networkId)
        {
            try
            {
                return Ok(bridgeData.GetSummaryReportMissingAttributes(simulationId, networkId, db));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/SummaryReport
        [HttpPost]
        [ModelValidation("Given simulation data is not valid")]
        public HttpResponseMessage Post([FromBody] SimulationModel simulationModel)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            MediaTypeHeaderValue mediaTypeHeaderValue = new MediaTypeHeaderValue("application/octet-stream");

            try
            {
                response.Content = new ByteArrayContent(summaryReportGenerator.GenerateExcelReport(simulationModel));
            }
            catch (TimeoutException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server has timed out. Please try again later.");
            }
            catch (InvalidOperationException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Invalid Operation: " + e.Message);
            }
            catch (OutOfMemoryException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server is out of Memory. Please try again later.");
            }

            response.Content.Headers.ContentType = mediaTypeHeaderValue;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "SummaryReport.xlsx"
            };

            return response;
        }
    }
}