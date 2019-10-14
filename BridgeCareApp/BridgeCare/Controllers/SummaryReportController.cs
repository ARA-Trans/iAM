using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class SummaryReportController : ApiController
    {
        private readonly IBridgeData repo;
        private readonly BridgeCareContext db;
        private readonly ISummaryReportGenerator summaryReportGenerator;

        public SummaryReportController(IBridgeData repo, BridgeCareContext db, ISummaryReportGenerator summaryReportGenerator)
        {
            this.repo = repo;
            this.db = db;
            this.summaryReportGenerator = summaryReportGenerator;
        }

        /// <summary>
        /// API endpoint for fetching a simulation's missing attributes for a summary report
        /// </summary>
        /// <param name="simulationId">Simulation identifier</param>
        /// <param name="networkId">Network identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetSummaryReportMissingAttributes")]
        public IHttpActionResult GetSummaryReportMissingAttributes(int simulationId, int networkId) =>
            Ok(repo.GetSummaryReportMissingAttributes(simulationId, networkId, db));

        /// <summary>
        /// API endpoint for fetching simulation data for a summary report
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/GetSummaryReport")]
        [ModelValidation("The scenario data is invalid.")]
        public HttpResponseMessage GetSummaryReport([FromBody] SimulationModel model)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(summaryReportGenerator.GenerateExcelReport(model));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "SummaryReport.xlsx"
            };

            return response;
            //return Ok(response);
        }
    }
}