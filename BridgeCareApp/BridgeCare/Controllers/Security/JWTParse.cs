using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace BridgeCare.Controllers
{
    public static class JWTParse
    {
        private static readonly RsaSecurityKey ESECPublicKey = GetPublicKey();

        public static Dictionary<string, string> GetUserInfo(string idToken)
        {
            JwtSecurityToken decodedToken = DecodeToken(idToken);
            return new Dictionary<string, string>
            {
                ["role"] = ParseLDAP(decodedToken.GetClaimValue("roles")),
                ["username"] = ParseLDAP(decodedToken.GetClaimValue("sub"))
            };
        }

        /// <summary>
        /// Retrieves the value of the claim of the given type from the JWT payload claims.
        /// </summary>
        /// <returns></returns>
        private static string GetClaimValue(this JwtSecurityToken jwt, string type)
        {
            return jwt.Claims.First(claim => claim.Type == type).Value;
        }

        /// <summary>
        /// Creates a JwtSecurityToken object from a JWT string.
        /// </summary>
        /// <param name="idToken">JWT string</param>
        private static JwtSecurityToken DecodeToken(string idToken)
        {
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
        private static RsaSecurityKey GetPublicKey()
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

                Task<HttpResponseMessage> responseTask = client.GetAsync("jwks?AuthorizationProvider=OIDC Auth Provider");
                responseTask.Wait();

                string resultJSON = responseTask.Result.Content.ReadAsStringAsync().Result;

                // This dictionary and list structure matches the structure of the JSON response from the ESEC JWKS endpoint
                var resultDictionary = (new JavaScriptSerializer()).Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(resultJSON);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(new RSAParameters()
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(resultDictionary["keys"][0]["n"]),
                    Exponent = Base64UrlEncoder.DecodeBytes(resultDictionary["keys"][0]["e"])
                });
                return new RsaSecurityKey(rsa);
            }
        }

        /// <summary>
        /// Given an LDAP-formatted string from ESEC, extracts the Common Name (CN) field.
        /// </summary>
        /// <param name="roleResponse">LDAP-formatted response</param>
        /// <returns>Role</returns>
        private static string ParseLDAP(string ldap)
        {
            string firstSegment = ldap.Split(',')[0];
            string role = firstSegment.Substring(3);
            return role;
        }
    }
}