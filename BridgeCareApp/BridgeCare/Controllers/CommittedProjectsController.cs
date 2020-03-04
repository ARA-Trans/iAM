using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Data;
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
        [RestrictAccess]
        public IHttpActionResult SaveCommittedProjectsFiles()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new ConstraintException("The data provided is not a valid MIME type.");

            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            repo.SaveCommittedProjectsFiles(HttpContext.Current.Request, db, userInformation);
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
        [RestrictAccess]
        public HttpResponseMessage ExportCommittedProjects([FromBody]SimulationModel model)
        {
            var response = Request.CreateResponse();
            var userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            byte[] byteArray = repo.ExportCommittedProjects(model.simulationId, model.networkId, db, userInformation);
            response.Content = new ByteArrayContent(byteArray);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CommittedProjects.xlsx"
            };

            return response;
            //return Ok(response);
        }
    }
}
