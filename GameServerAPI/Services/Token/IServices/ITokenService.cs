namespace GameServerAPI.Services.Token.IServices
{
    /// <summary>
    /// Defines methods for a service that creates and validates tokens
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITokenService<T> where T : class
    {
        /// <summary>
        /// Creates a JWT Token based on the provided token information.
        /// </summary>
        /// <param name="customToken">The token information used to create the JWT token.</param>
        /// <returns>A string representing the JWT token.</returns>
        string CreateToken(T customToken);

        /// <summary>
        /// Validates given JWT token and extracts the token information
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>An instance of the custom token type if the validation is successful. If unsuccessful returns null.</returns>
        T? ValidateToken(string token);

    }
}
