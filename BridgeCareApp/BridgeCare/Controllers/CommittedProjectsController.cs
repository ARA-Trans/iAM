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

        [HttpPost]
        [Route("api/SaveCommittedProjectsFiles")]
        public IHttpActionResult SaveCommittedProjectsFiles()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new ConstraintException("The data provided is not a valid MIME type.");

            repo.SaveCommittedProjectsFiles(HttpContext.Current.Request, db);
            return Ok();
        }

        [HttpPost]
        [Route("api/ExportCommittedProjects")]
        [ModelValidation("The simulation data is not valid.")]
        public IHttpActionResult Post([FromBody]SimulationModel simulationModel)
        {
            var response = Request.CreateResponse();
            response.Content = new ByteArrayContent(repo.ExportCommittedProjects(simulationModel.SimulationId, simulationModel.NetworkId, db));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CommittedProjects.xlsx"
            };

            return Ok(response);
        }
    }
}