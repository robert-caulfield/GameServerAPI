using Microsoft.IdentityModel.Tokens;
using GameServerAPI.Models.Token;
using GameServerAPI.Models.TokenObjects;
using System.Data;
using System.Security.Claims;

namespace GameServerAPI.Services.Token
{

    /// <summary>
    /// Provides services for creating and validating game server tokens.
    /// </summary>
    public class AuthTokenService : BaseTokenService<AuthToken>
    {
        // Time in hours that the token will expire
        const double EXPIRATION_TIME = 12.0;

        /// <summary>
        /// Initializes instance of the AuthTokenService class for managing the creation
        /// and validation of game server tokens.
        /// </summary>
        /// <param name="SecretKey">The secrete used for token generation and validation. Provided via
        /// dependency injection.</param>
        public AuthTokenService(string SecretKey) : base(SecretKey)
        {
        }

        /// <summary>
        /// Creates a token descriptor for a auth token.
        /// </summary>
        /// <param name="customToken">The auth token containing the claims to be included in the JWT.</param>
        /// <returns>A SecurityTokenDescriptor configured with the provided claims and other token settings.</returns>
        protected override SecurityTokenDescriptor CreateTokenDescriptor(AuthToken customToken)
        {
            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, customToken.NamedId),
                    new Claim(ClaimTypes.Role, customToken.Role)
                }),
                SigningCredentials = new(new SymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(EXPIRATION_TIME)
            };
        }

        /// <summary>
        /// Extracts claims from ClaimsPrincipal and use it to populate a GameServerToken object.
        /// </summary>
        /// <param name="claimsPrincipal">The ClaimsPrincipal containing the claims to extract.</param>
        /// <returns>A GameServerToken object populated with the extracted claims.</returns>
        protected override AuthToken ExtractClaims(ClaimsPrincipal claimsPrincipal)
        {
            return new AuthToken
            {
                NamedId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
                Role = claimsPrincipal.FindFirstValue(ClaimTypes.Role) ?? ""
            };
        }
    }
}
