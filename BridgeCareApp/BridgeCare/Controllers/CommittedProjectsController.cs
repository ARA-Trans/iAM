using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
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

            committedProjects.SaveCommittedProjectsFiles(httpRequest, db);

            return Ok();
        }

        [Route("api/ExportCommittedProjects")]
        [ModelValidation("Given simulation data is not valid")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]SimulationModel simulationModel)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            var mediaTypeHeaderValue = new MediaTypeHeaderValue("application/octet-stream");
            try
            {
                response.Content = new ByteArrayContent(committedProjects.ExportCommittedProjects(simulationModel.SimulationId, simulationModel.NetworkId, db));
            }
            catch (TimeoutException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server has timed out. Please try again later.");
            }
            catch (OutOfMemoryException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The server is out of Memory. Please try again later.");
            }

            response.Content.Headers.ContentType = mediaTypeHeaderValue;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CommittedProjects.xlsx"
            };

            return response;
        }
    }
}