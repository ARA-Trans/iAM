using System;
using System.Configuration;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BridgeCare.Controllers
{
    [RoutePrefix("auth")]
    public class WindowsAuthorizationController : ApiController
    {
        /// <summary>
        /// API endpoint for authenticating users using windows identity
        /// Throws an AuthenticationException if a user is not authenticated or cannot be verified
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("AuthenticateUser")]
        [Authorize]
        [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
        public IHttpActionResult GetUser()
        {
            if (!User.Identity.IsAuthenticated)
                throw new AuthenticationException("User failed authentication.");

            var windowsIdentity = HttpContext.Current.Request.LogonUserIdentity;

            if (windowsIdentity == null)
                throw new AuthenticationException("System cannot determine user identity.");

            return Ok(new UserInformationModel(windowsIdentity));
        }

        /// <summary>
        /// API endpoint for fetching ID and Access tokens from ESEC using the OpenID Connect protocol
        /// </summary>
        /// <param name="code">The authentication or error code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserTokens/{code}")]
        public IHttpActionResult GetUserTokens(string code)
        {
            var esecConfig = (NameValueCollection)ConfigurationManager.GetSection("ESECConfig");
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(esecConfig["ESECBaseAddress"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            formData.Add(new KeyValuePair<string, string>("code", WebUtility.UrlDecode(code)));
            formData.Add(new KeyValuePair<string, string>("redirect_uri", esecConfig["ESECRedirect"]));
            formData.Add(new KeyValuePair<string, string>("client_id", esecConfig["ESECClientId"]));
            formData.Add(new KeyValuePair<string, string>("client_secret", esecConfig["ESECClientSecret"]));
            HttpContent content = new FormUrlEncodedContent(formData);

            Task<HttpResponseMessage> responseTask = client.PostAsync("token", content);
            responseTask.Wait();

            String response = responseTask.Result.Content.ReadAsStringAsync().Result;

            return Ok(response);
        }
    }
}
