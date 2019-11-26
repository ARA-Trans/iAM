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
    public class AuthenticationController : ApiController
    {
        /// <summary>
        /// API endpoint for fetching user info from ESEC using the OpenID Connect protocol
        /// </summary>
        /// <param name="token">The user's access token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserInfo/{token}")]
        public IHttpActionResult GetUserInfo(string token)
        {
            return Ok(GetUserInfoString(token));
        }

        public static string GetUserInfoString(string token)
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
            formData.Add(new KeyValuePair<string, string>("access_token", WebUtility.UrlDecode(token)));
            HttpContent content = new FormUrlEncodedContent(formData);

            Task<HttpResponseMessage> responseTask = client.PostAsync("userinfo", content);
            responseTask.Wait();

            return responseTask.Result.Content.ReadAsStringAsync().Result;
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

            string response = responseTask.Result.Content.ReadAsStringAsync().Result;

            return Ok(response);
        }

        [HttpPost]
        [Route("RevokeToken/{token}")]
        public IHttpActionResult RevokeToken(string token)
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
            formData.Add(new KeyValuePair<string, string>("client_id", esecConfig["ESECClientId"]));
            formData.Add(new KeyValuePair<string, string>("client_secret", esecConfig["ESECClientSecret"]));
            HttpContent content = new FormUrlEncodedContent(formData);

            string query = $"?token={WebUtility.UrlDecode(token)}";

            Task<HttpResponseMessage> responseTask = client.PostAsync("revoke" + query, content);
            responseTask.Wait();
            string response = responseTask.Result.Content.ReadAsStringAsync().Result;

            return Ok(response);
        }
    }
}
