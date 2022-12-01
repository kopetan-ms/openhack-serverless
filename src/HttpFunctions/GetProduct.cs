using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Openhack.MS
{
    public class GetProduct
    {
        private readonly ILogger _logger;

        public GetProduct(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetProduct>();
        }

        [Function("GetProduct")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var productId = queryParams["productId"];
            response.WriteString($"The product name for your product id {productId} is Starfruit Explosion");
            return response;
        }
    }
}
