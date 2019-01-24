using BridgeCareCodeFirst.Models;
using BridgeCareCodeFirst.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace BridgeCareCodeFirst.Controllers
{
    public class DetailedReportController : ApiController
    {
        DetailedReportRepository detailedReportRepository = new DetailedReportRepository();

        // GET: api/DetailedReport
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DetailedReport/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DetailedReport
        [HttpPost]
        public HttpResponseMessage Post([FromBody] ReportDataModel data)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Given Network Id and/or Simulation Id are not valid");
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/octet-stream");
            try
            {
                response.Content = new ByteArrayContent(detailedReportRepository.CreateExcelReport(data));
            }
            catch (TimeoutException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server has timed out. Please try after some time");
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Selected Network table and/or simulation table are not present in the database");
            }
            catch (OutOfMemoryException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server is out of Memory. Please try after some time");
            }
            response.Content.Headers.ContentType = mediaType;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "DetailedReport.xlsx";
            return response;
        }
    }
}
