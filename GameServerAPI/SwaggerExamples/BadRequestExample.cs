using GameServerAPI.Models.API;
using GameServerAPI.Models.DTO.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace GameServerAPI.SwaggerExamples
{
    public class BadRequestExample : IExamplesProvider<APIResponse>
    {
        public APIResponse GetExamples()
        {
            return new APIResponse
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Errors = new List<string> { "Invalid information provided" }
            };
        }
    }
}
