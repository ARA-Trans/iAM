using AspWebApi.Models;
using AspWebApi.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace AspWebApi.Controllers
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
        public HttpResponseMessage Post([FromBody] ReportData data)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Given Network Id and/or Simulation Id are not valid");
            }
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/octet-stream");
            try
            {
                response.Content = new ByteArrayContent(detailedReportRepository.GetDetailedReportData(data));
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Network and/or simulation tables are not present in the database");

            }
            response.Content.Headers.ContentType = mediaType;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "DetailedReport.xlsx";
            return response;
        }

        // PUT: api/DetailedReport/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DetailedReport/5
        public void Delete(int id)
        {
        }
    }
}
