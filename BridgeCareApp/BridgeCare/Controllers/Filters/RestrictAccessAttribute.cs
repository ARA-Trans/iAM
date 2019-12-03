using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Net;
using System.Web.Script.Serialization;

namespace BridgeCare.Controllers.Filters
{
    /// <summary>
    /// Restricts access to an API endpoint, only allowing requests with a valid access token to be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RestrictAccessAttribute : AuthorizeAttribute
    {
        private string[] PermittedRoles { get; set; }
        private static RsaSecurityKey ESECPublicKey { get; set; }

        /// <summary>
        /// Only users with the provided roles will be able to access the endpoint.
        /// If no roles are listed, all authenticated users will be able to access it.
        /// </summary>
        /// <param name="roles">Permitted roles</param>
        public RestrictAccessAttribute (params string[] roles) : base()
        {
            this.PermittedRoles = roles;
        }

        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            if (!TryGetAuthorization(httpContext.Request.Headers, out string idToken))
            {
                return false;
            }

            JwtSecurityToken decodedToken = DecodeToken(idToken);

            Claim roleClaim = decodedToken.Claims.First(claim => claim.Type == "roles");

            string role = ParseRoleResponse(roleClaim.Value);
            
            if (PermittedRoles.Length == 0)
            {
                return true;
            }

            return PermittedRoles.Contains(role);
        }

        /// <summary>
        /// Creates a JwtSecurityToken object from a JWT string.
        /// </summary>
        /// <param name="idToken">JWT string</param>
        private JwtSecurityToken DecodeToken(string idToken)
        {
            if (ESECPublicKey == null)
            {
                GetPublicKey();
            }
            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = ESECPublicKey
            };

            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(idToken, validationParameters, out SecurityToken validatedToken);
            return validatedToken as JwtSecurityToken;
        }

        /// <summary>
        /// Fetches the public key information from the ESEC jwks endpoint, and generates an RsaSecurityKey from it
        /// </summary>
        private static void GetPublicKey()
        {
            var esecConfig = (NameValueCollection)ConfigurationManager.GetSection("ESECConfig");
            // These two lines should be removed as soon as the ESEC site's certificates start working
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(esecConfig["ESECBaseAddress"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Task<HttpResponseMessage> responseTask = client.GetAsync("jwks?AuthorizationProvider=OIDC Auth Provider");
            responseTask.Wait();

            string resultJSON = responseTask.Result.Content.ReadAsStringAsync().Result;

            var resultDictionary = (new JavaScriptSerializer()).Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(resultJSON);
            
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(new RSAParameters()
            {
                Modulus = Base64UrlEncoder.DecodeBytes(resultDictionary["keys"][0]["n"]),
                Exponent = Base64UrlEncoder.DecodeBytes(resultDictionary["keys"][0]["e"])
            });
            ESECPublicKey = new RsaSecurityKey(rsa);
        }

        /// <summary>
        /// Given the LDAP-formatted "roles" string from ESEC, extracts the role
        /// </summary>
        /// <param name="roleResponse">LDAP-formatted response</param>
        /// <returns>Role</returns>
        private static string ParseRoleResponse(string roleResponse)
        {
            string firstSegment = roleResponse.Split(',')[0];
            string role = firstSegment.Substring(3);
            return role;
        }

        /// <summary>
        /// Attempts to get an authorization parameter from an HTTP request's headers
        /// </summary>
        /// <param name="headers">Request headers</param>
        /// <returns>Returns true if successful</returns>
        private static bool TryGetAuthorization(HttpRequestHeaders headers, out string authorization)
        {
            if (!headers.Contains("Authorization"))
            {
                authorization = "";
                return false;
            }
            if (headers.Authorization.Parameter == null)
            {
                authorization = "";
                return false;
            }
            authorization = headers.Authorization.Parameter.ToString();
            return true;
        }
    }
}