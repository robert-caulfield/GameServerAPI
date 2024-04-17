using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GameServerAPI.Models.API;
using System.Net;

namespace GameServerAPI.Utilities
{
    /// <summary>
    /// Utility class for common tasks in API controllers
    /// </summary>
    public class ApiControllerUtilities
    {
        /// <summary>
        /// Constructs a HTTP 200 OK response with a specified result
        /// </summary>
        /// <typeparam name="T">The type of result</typeparam>
        /// <param name="result">The result included in response</param>
        /// <returns>An HTTP 200 OK response with the included result wrapped in a <see cref="APIResponse"/></returns>
        public static ActionResult<APIResponse> OkResult<T>(T result)
        {
            return new ObjectResult(
            new APIResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = result
            }
            )
            { StatusCode = (int)HttpStatusCode.OK };
        }
        /// <summary>
        /// Constructs an error response with a specified status code and single error message
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the desired response</param>
        /// <param name="errorMessage">The error message to be included in the response</param>
        /// <returns>An <see cref="ObjectResult"/> representing an error response containing a <see cref="APIResponse"/></returns>
        public static ActionResult<APIResponse> HandleError(HttpStatusCode statusCode, string errorMessage)
        {
            return HandleError(statusCode, new List<string> {errorMessage});
        }

        /// <summary>
        /// Constructs an error response with a specified status code and list of errors
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the desired response</param>
        /// <param name="errorMessages">A list of error messages to be included in response</param>
        /// <returns>An <see cref="ObjectResult"/> representing an error response containing a <see cref="APIResponse"/></returns>
        public static ActionResult<APIResponse> HandleError(HttpStatusCode statusCode, List<string> errorMessages)
        {
            return new ObjectResult(
            new APIResponse()
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = errorMessages
            }
            )
            { StatusCode = (int)statusCode };
        }

        /// <summary>
        /// Constructs an error response from a given model state.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="modelState">Provided model state.</param>
        /// <param name="customErrors">Custom errors to add to APIResponse.Erros</param>
        /// <returns></returns>
        public static ActionResult<APIResponse> HandleInvalidModelState(HttpStatusCode statusCode, ModelStateDictionary modelState, List<string> customErrors = null)
        {
            // Create error list
            List<string> errors = new List<string>();
            // Append custom errors
            if (customErrors != null)
            {
                errors.AddRange(customErrors);
            }
            // Append model state errors
            errors.AddRange (modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            // Return error response
            return HandleError (statusCode, errors);
        }
    
    }
}
