using System;
using System.Collections.Concurrent;
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

namespace BridgeCare.Security
{
    public static class JWTParse
    {
        private static readonly RsaSecurityKey ESECPublicKey = GetPublicKey();
        /// <summary>
        /// Each key is a token that has been revoked. Its value is the unix timestamp of the time at which it expires.
        /// </summary>
        private static ConcurrentDictionary<string, long> revokedTokens = new ConcurrentDictionary<string, long>();

        /// <summary>
        /// Checks if the provided token has been revoked.
        /// </summary>
        /// <param name="idToken">JWT ID Token</param>
        /// <returns>bool</returns>
        private static bool TokenIsRevoked(string idToken)
        {
            return revokedTokens.ContainsKey(idToken);
        }

        /// <summary>
        /// Prevents the parser from accepting the provided token in the future.
        /// </summary>
        /// <param name="idToken">The JWT ID Token</param>
        public static void RevokeToken(string idToken)
        {
            RemoveExpiredTokens();
            JwtSecurityToken decodedToken = DecodeToken(idToken);
            string expirationString = decodedToken.GetClaimValue("exp");
            long expiration = Int64.Parse(expirationString);
            revokedTokens.TryAdd(idToken, expiration);
        }

        /// <summary>
        /// Removes all expired tokens from the revokedTokens dictionary, as they no longer need to be tracked.
        /// This keeps the dictionary from endlessly growing as the application runs
        /// </summary>
        private static void RemoveExpiredTokens()
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            revokedTokens = new ConcurrentDictionary<string, long>(revokedTokens.Where(entry => entry.Value > currentTime));
        }

        /// <summary>
        /// Given an id_token from ESEC, validates it and extracts the User's Information
        /// </summary>
        /// <param name="idToken">JWT id_token from Authorization Header</param>
        /// <returns></returns>
        public static Models.UserInformationModel GetUserInformation(string idToken)
        {
            if (TokenIsRevoked(idToken))
                throw new UnauthorizedAccessException("Your ID Token has been revoked.");
            JwtSecurityToken decodedToken = DecodeToken(idToken);
            List<string> roleStrings = ParseLDAP(decodedToken.GetClaimValue("roles"));
            if (roleStrings.Count == 0)
                throw new UnauthorizedAccessException("User has no security roles assigned.");
            string role = roleStrings.Where(roleString => Role.AllValidRoles.Contains(roleString)).First();
            string name = ParseLDAP(decodedToken.GetClaimValue("sub"))[0];
            string email = decodedToken.GetClaimValue("email");
            return new Models.UserInformationModel(name, role, email);
        }

        /// <summary>
        /// Retrieves the value of the claim of the given type from the JWT payload claims.
        /// </summary>
        /// <returns></returns>
        private static string GetClaimValue(this JwtSecurityToken jwt, string type)
        {
            return jwt.Claims.FirstOrDefault(claim => claim.Type == type).Value;
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
        /// Given an LDAP-formatted string from ESEC, extracts the Common Name (CN) fields.
        /// </summary>
        /// <param name="roleResponse">LDAP-formatted response</param>
        /// <returns>Role</returns>
        private static List<string> ParseLDAP(string ldap)
        {
            string[] segments = ldap.Split('^');
            List<string> commonNames = new List<string>();
            foreach (string segment in segments)
            {
                string firstSubSegment = segment.Split(',')[0];
                string commonName = firstSubSegment.Split('=')[1];
                commonNames.Add(commonName);
            }
            return commonNames;
        }
    }
}
