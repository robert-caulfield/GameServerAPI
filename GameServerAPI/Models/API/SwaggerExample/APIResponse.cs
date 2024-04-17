using System.Net;

namespace GameServerAPI.Models.API.SwaggerExample
{
    /// CLASS USED FOR SWAGGER UI TO SHOW CORRECT APIResponse.Result STRUCTURE!"/>
    /// <summary>
    /// Standardized response for API endpoints.
    /// </summary>
    public class APIResponse<T>
    {
        /// <summary>The HTTP status code of response.</summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>Indicates whether request was successful.</summary>
        public bool IsSuccess { get; set; } = true;

        /// <summary>List of errors, if any, describing problems with the request. Only populated when request is unsuccessful.</summary>
        public List<string> Errors { get; set; } = [""];

        /// <summary>The result of the request. Can be null if there is nothing to return or if the request is unsuccessful.</summary>
        public T Result { get; set; }
    }
}
