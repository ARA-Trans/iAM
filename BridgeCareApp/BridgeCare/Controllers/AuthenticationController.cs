using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace BridgeCare.Controllers
{
    [RoutePrefix("authentication")]
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
            var response = GetUserInfoString(token);
            ValidateResponse(response);
            return Ok(response);
        }

        /// <summary>
        /// Fetches user info as a JSON-formatted string from ESEC
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>JSON-formatted user info</returns>
        public static string GetUserInfoString(string token)
        {
            var esecConfig = (NameValueCollection)ConfigurationManager.GetSection("ESECConfig");
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (HttpClient client = new HttpClient(handler))
            {
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
        }

        /// <summary>
        /// Fetches user info as a dictionary
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>User info dictionary</returns>
        public static Dictionary<string,string> GetUserInfoDictionary(string token)
        {
            return DictionaryFromJSON(GetUserInfoString(token));
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

            using (HttpClient client = new HttpClient(handler))
            {
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

                ValidateResponse(response);

                return Ok(response);
            }
        }

        /// <summary>
        /// Sends a refresh token to ESEC, returning a new Access Token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken/{refreshToken}")]
        public IHttpActionResult GetRefreshToken(string refreshToken)
        {
            var esecConfig = (NameValueCollection)ConfigurationManager.GetSection("ESECConfig");
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(esecConfig["ESECBaseAddress"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formData = new List<KeyValuePair<string, string>>();
                formData.Add(new KeyValuePair<string, string>("client_id", esecConfig["ESECClientId"]));
                formData.Add(new KeyValuePair<string, string>("client_secret", esecConfig["ESECClientSecret"]));
                HttpContent content = new FormUrlEncodedContent(formData);

                string query = $"?grant_type=refresh_token&refresh_token={WebUtility.UrlDecode(refreshToken)}";

                Task<HttpResponseMessage> responseTask = client.PostAsync("token" + query, content);
                responseTask.Wait();

                string response = responseTask.Result.Content.ReadAsStringAsync().Result;

                ValidateResponse(response);

                return Ok(response);
            }
        }

        /// <summary>
        /// Sends an access or refresh token to the revocation endpoint, preventing the token from ever being used again.
        /// </summary>
        /// <param name="token">Access or Refresh Token</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RevokeToken/Access/{token}")]
        [Route("RevokeToken/Refresh/{token}")]
        public IHttpActionResult RevokeToken(string token)
        {
            var esecConfig = (NameValueCollection)ConfigurationManager.GetSection("ESECConfig");
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            using (HttpClient client = new HttpClient(handler))
            {
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
                if (responseTask.Result.StatusCode == HttpStatusCode.OK)
                {
                    return Ok();
                }

                string response = responseTask.Result.Content.ReadAsStringAsync().Result;
                ValidateResponse(response);

                return Ok(response);
            }
        }

        /// <summary>
        /// Prevents an id token from being accepted by the application again.
        /// ID tokens are not validated by the ESEC server, so they cannot be invalidated in the
        /// same way as refresh or access tokens. Instead, we must locally keep track of them.
        /// </summary>
        [HttpPost]
        [Route("RevokeToken/Id")]
        public IHttpActionResult RevokeIdToken()
        {
            // A JWT is too large to store in the URL, so it is passed in the authorization header.
            string idToken = Request.Headers.Authorization.Parameter;
            JWTParse.RevokeToken(idToken);
            return Ok();
        }

        /// <summary>
        /// Converts a JSON-formatted string into a Dictionary
        /// </summary>
        /// <param name="jsonString">JSON-formatted string</param>
        /// <returns>The JSON object as a dictionary</returns>
        private static Dictionary<string,string> DictionaryFromJSON(string jsonString)
        {
            return (new JavaScriptSerializer()).Deserialize<Dictionary<string, string>>(jsonString);
        }

        /// <summary>
        /// Checks to ensure that a response from the ESEC OIDC endpoint is not an error.
        /// </summary>
        /// <param name="response">The JSON-formatted response string</param>
        private static void ValidateResponse(string response)
        {
            var responseJSON = DictionaryFromJSON(response);
            if (responseJSON.ContainsKey("error"))
            {
                throw new AuthenticationException(responseJSON["error_description"]);
            }
        }
    }
}
