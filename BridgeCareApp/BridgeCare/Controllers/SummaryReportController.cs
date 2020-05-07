using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using Hangfire;
using System;
using System.IO;
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

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SummaryReportController));

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
        [RestrictAccess]
        public IHttpActionResult GetSummaryReportMissingAttributes(int simulationId, int networkId) =>
            Ok(repo.GetSummaryReportMissingAttributes(simulationId, networkId, db));

        /// <summary>
        /// API endpoint for fetching simulation data for a summary report
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/GenerateSummaryReport")]
        [ModelValidation("The scenario data is invalid.")]
        [RestrictAccess]
        public HttpResponseMessage GenerateSummaryReport([FromBody] SimulationModel model)
        {
            BackgroundJob.Enqueue(() => summaryReportGenerator.GenerateExcelReport(model));
            var response = Request.CreateResponse(HttpStatusCode.OK, "Report generation started");
            return response;
        }

        [HttpPost]
        [Route("api/DownloadSummaryReport")]
        [ModelValidation("The scenario data is invalid.")]
        [RestrictAccess]
        public HttpResponseMessage DownloadSummaryReport([FromBody] SimulationModel model)
        {
            var folderPath = $"DownloadedReports\\{model.simulationId}";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "SummaryReport.xlsx");
            var response = new HttpResponseMessage();
            if (!File.Exists(filePath))
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, $"Summary report is not available in the path {filePath}");
                log.Error($"Summary report is not available in the path {filePath}");
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(summaryReportGenerator.DownloadExcelReport(model));
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "SummaryReport.xlsx"
            };
            return response;
        }

        [HttpGet]
        [Route("api/GetJobList")]
        [ModelValidation("Something went wrong on GetJobList API")]
        [RestrictAccess]
        public HttpResponseMessage GetJobList()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK, "Access granted");
            return response;
        }
    }
}
