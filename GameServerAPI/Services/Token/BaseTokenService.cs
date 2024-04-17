using Microsoft.IdentityModel.Tokens;
using GameServerAPI.Services.Token.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameServerAPI.Services.Token
{
    /// <summary>
    /// Provides base implementation for token services  that can create and validate JWT tokens.
    /// </summary>
    /// <typeparam name="T">The type of the custom token object that will be used to populate token claims.</typeparam>
    public abstract class BaseTokenService<T> : ITokenService<T> where T : class
    {
        /// <summary>
        /// The secret key stored as a byte array.
        /// </summary>
        protected readonly byte[] SecretKey;

        /// <summary>
        /// Initializes instance of the BaseTokenService class.
        /// </summary>
        /// <param name="SecretKey">The secret key used for token generation and validation.
        /// This key is provided via dependency injection.</param>
        protected BaseTokenService(string SecretKey)
        {
            // Convert the secret key from string to byte array
            this.SecretKey = Encoding.UTF8.GetBytes(SecretKey);
        }

        /// <summary>
        /// Creates a JWT Token based on the provided token properties and claims.
        /// defined in <see cref="CreateTokenDescriptor(T)"/>.
        /// </summary>
        /// <param name="customToken">The token information used to create the JWT token.</param>
        /// <returns>A string representing the JWT token.</returns>
        public string CreateToken(T customToken)
        {
            SecurityTokenDescriptor tokenDescriptor = CreateTokenDescriptor(customToken);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        /// <summary>
        /// Validates given JWT token using validation parameters defined in <see cref="CreateValidationParameters"/>
        /// and extracts the token information using <see cref="ExtractClaims(ClaimsPrincipal)"/>.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>An instance of the custom token type if the validation is successful. If unsuccessful returns null.</returns>
        public T? ValidateToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = CreateValidationParameters();
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                T customToken = ExtractClaims(claimsPrincipal);
                return customToken;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Creates and returns token validation parameters using the secret key provided during service instantiation.
        /// </summary>
        /// <returns>TokenValidationParameters signed with the secret key.</returns>
        protected virtual TokenValidationParameters CreateValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                ClockSkew = TimeSpan.FromMinutes(0)
            };
        }
        /// <summary>
        /// Method for creating and returning token descriptor for a given custom token.
        /// </summary>
        /// <param name="customToken">The object used to populate the claims of the token.</param>
        /// <returns></returns>
        protected abstract SecurityTokenDescriptor CreateTokenDescriptor(T customToken);

        /// <summary>
        /// Extracts and populates a custom token object with the claims defined in
        /// given ClaimsPrincipal.
        /// </summary>
        /// <param name="claimsPrincipal">The ClaimsPrincipal containing claims to be extracted.</param>
        /// <returns>A custom token object populated with the extracted claims.</returns>
        protected abstract T ExtractClaims(ClaimsPrincipal claimsPrincipal);

    }
}
