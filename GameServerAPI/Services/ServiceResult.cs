using System.Net;

namespace GameServerAPI.Services
{
    /// <summary>
    /// Represents the result of a service operation with a generic result type.
    /// </summary>
    /// <typeparam name="T">The type of the result object.</typeparam>
    public class ServiceResult<T> where T : class
    {
        /// <summary>
        /// Result of the service operatrion.
        /// This property will be null if the operation was not successful.
        /// </summary>
        public T? Result { get; set; }

        /// <summary>
        /// List of error messages that occured during registration. 
        /// If registration was successful, this list should be empty.
        /// </summary>
        public List<String> Errors { get; set; } = new List<String>();

        /// <summary>
        /// Value indicating if the registration was successful.
        /// The registration is considered successful if <see cref="Result"/> is not null
        /// </summary>
        public bool IsSuccess => Result != null;

        /// <summary>
        /// HTTP status code associated with the service operation.
        /// Defaults to HttpStatusCode.OK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK; 
    }
}
