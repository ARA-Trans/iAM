using AspWebApi.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public IHttpActionResult Post([FromBody] ReportData data)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            detailedReportRepository.GetDetailedReportData(data);
            return Ok();
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
    public class ReportData
    {
        public int NetworkId;
        public int SimulationId;
    }
}
