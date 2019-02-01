using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class DetailedReportController : ApiController
    {
        private readonly IReportCreator detailedReport;
        public DetailedReportController(IReportCreator detailedReport)
        {
            this.detailedReport = detailedReport ?? throw new ArgumentNullException(nameof(detailedReport));
        }

        // POST: api/DetailedReport
        [HttpPost]
        public HttpResponseMessage Post([FromBody] SimulationModel data)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Given Network Id and/or Simulation Id are not valid");
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/octet-stream");
            try
            {
                response.Content = new ByteArrayContent(detailedReport.CreateExcelReport(data));
            }
            catch (TimeoutException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server has timed out. Please try again later");
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Selected Network table and/or simulation table are not present in the database");
            }
            catch (OutOfMemoryException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server is out of Memory. Please try again later");
            }
            response.Content.Headers.ContentType = mediaType;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "DetailedReport.xlsx";
            return response;
        }
    }
}
