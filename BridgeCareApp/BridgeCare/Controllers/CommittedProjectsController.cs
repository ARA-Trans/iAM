using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class CommittedProjectsController : ApiController
    {
        private readonly ICommittedProjects repo;
        private readonly BridgeCareContext db;

        public CommittedProjectsController(ICommittedProjects repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for saving committed projects data
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveCommittedProjectsFiles")]
        public IHttpActionResult SaveCommittedProjectsFiles()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new ConstraintException("The data provided is not a valid MIME type.");

            repo.SaveCommittedProjectsFiles(HttpContext.Current.Request, db);
            return Ok();
        }

        /// <summary>
        /// API endpoint for creating committed projects excel file
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/ExportCommittedProjects")]
        [ModelValidation("The scenario data is invalid.")]
        public IHttpActionResult ExportCommittedProjects([FromBody]SimulationModel model)
        {
            var response = Request.CreateResponse();
            response.Content = new ByteArrayContent(repo.ExportCommittedProjects(model.SimulationId, model.NetworkId, db));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CommittedProjects.xlsx"
            };

            return Ok(response);
        }
    }
}