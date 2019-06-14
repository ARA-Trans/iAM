using BridgeCare.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class CommittedProjectsController: ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ICommittedProjects committedProjects;

        public CommittedProjectsController(ICommittedProjects committedProjects, BridgeCareContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));        
            this.committedProjects = committedProjects ?? throw new ArgumentNullException(nameof(committedProjects));
        }
                
        [Route("api/SaveCommittedProjectsFiles")]
        [HttpPost]
        public IHttpActionResult Post()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count < 1)
            {
                return BadRequest();
            }

            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var selectedScenarioId = httpRequest.Form.Get("selectedScenarioId");
            var networkId = httpRequest.Form.Get("networkId");
            committedProjects.SaveCommittedProjectsFiles(httpRequest.Files, selectedScenarioId, networkId, db);
            return Ok();
        }
    }
}