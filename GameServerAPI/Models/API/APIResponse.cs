using System.Net;

namespace GameServerAPI.Models.API
{
    /// <summary>
    /// Standardized response for API endpoints.
    /// </summary>
    public class APIResponse
    {
        /// <summary>The HTTP status code of response.</summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>Indicates whether request was successful.</summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>List of errors, if any, describing problems with the request. Only populated when request is unsuccessful.</summary>
        public List<string> Errors { get; set; }

        /// <summary>The result of the request. Can be null if there is nothing to return or if the request is unsuccessful.</summary>
        public object Result { get; set; }
    }
}
