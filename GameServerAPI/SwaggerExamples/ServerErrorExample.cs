using GameServerAPI.Models.API;
using GameServerAPI.Models.DTO.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace GameServerAPI.SwaggerExamples
{
    public class ServerErrorExample : IExamplesProvider<APIResponse>
    {
        public APIResponse GetExamples()
        {
            return new APIResponse
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Errors = new List<string> { "A server error was encountered." }
            };
        }
    }
}
