using GameServerAPI.Models.API;
using GameServerAPI.Models.DTO.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace GameServerAPI.SwaggerExamples
{
    public class ForbiddenExample : IExamplesProvider<APIResponse>
    {
        public APIResponse GetExamples()
        {
            return new APIResponse
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.Forbidden,
                Errors = new List<string> { "Invalid authorization level to access resource." }
            };
        }
    }
}
