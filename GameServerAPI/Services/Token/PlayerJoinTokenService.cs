using Microsoft.IdentityModel.Tokens;
using GameServerAPI.Models.TokenObjects;
using System.Security.Claims;

namespace GameServerAPI.Services.Token
{
    /// <summary>
    /// Provides services for creating and validating player join tokens.
    /// </summary>
    public class PlayerJoinTokenService : BaseTokenService<PlayerJoinToken>
    {
        // Time in minutes that the token will expire
        const double EXPIRATION_TIME = 1.0;

        /// <summary>
        /// Initializes instance of the PlayerJoinTokenService class for managing the creation
        /// and validation of player join tokens.
        /// </summary>
        /// <param name="SecretKey">The secrete used for token generation and validation. Provided via
        /// dependency injection.</param>
        public PlayerJoinTokenService(string SecretKey) : base(SecretKey)
        {
        }

        /// <summary>
        /// Creates a token descriptor for a player join token.
        /// </summary>
        /// <param name="customToken">The player join token containing the claims to be included in the JWT.</param>
        /// <returns>A SecurityTokenDescriptor configured with the provided claims and other token settings.</returns>
        protected override SecurityTokenDescriptor CreateTokenDescriptor(PlayerJoinToken customToken)
        {
            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", customToken.Id),
                    new Claim("serverid", customToken.ServerId),

                }),
                Expires = DateTime.UtcNow.AddMinutes(EXPIRATION_TIME),
                SigningCredentials = new(new SymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature)
                
            };
        }

        /// <summary>
        /// Extracts claims from ClaimsPrincipal and use it to populate a PlayerJoinToken object.
        /// </summary>
        /// <param name="claimsPrincipal">The ClaimsPrincipal containing the claims to extract.</param>
        /// <returns>A PlayerJoinToken object populated with the extracted claims.</returns>
        protected override PlayerJoinToken ExtractClaims(ClaimsPrincipal claimsPrincipal)
        {
            return new PlayerJoinToken
            {
                Id = claimsPrincipal.FindFirst("id")?.Value ?? "0",
                ServerId = claimsPrincipal.FindFirst("serverid")?.Value ?? "0",
            };
        }
    }
}
