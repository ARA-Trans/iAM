using BridgeCare.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class SummaryReportController : ApiController
    {
        private readonly ISummaryReportGenerator summaryReportGenerator;

        public SummaryReportController(ISummaryReportGenerator summaryReportGenerator)
        {
            this.summaryReportGenerator = summaryReportGenerator;
        }

        // POST: api/SummaryReport
        [HttpPost]
        public HttpResponseMessage Post([FromBody] SimulationModel simulationModel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Given Network Id and/or Simulation Id are not valid.");
            }

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
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Selected Network table and/or simulation table are not present in the database.");
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