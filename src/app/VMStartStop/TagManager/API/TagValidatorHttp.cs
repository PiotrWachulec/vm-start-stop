using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyCo
{
    public class TagValidatorHttp
    {
        private readonly ILogger<TagValidatorHttp> _logger;

        public TagValidatorHttp(ILogger<TagValidatorHttp> logger)
        {
            _logger = logger;
        }

        [Function("TagValidatorHttp")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
