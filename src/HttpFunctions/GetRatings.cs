using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Openhack.MS
{
    public class GetRatings
    {
        private readonly ILogger _logger;

        public GetRatings(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetRatings>();
        }

        [Function("GetRatings")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get",Route = "GetRatings/{userId}")] HttpRequestData req, [CosmosDBInput(databaseName: "Ratings",
                       collectionName: "Challenge3",
                       ConnectionStringSetting = "CosmosDbConnectionString",
                       SqlQuery = "select * from Ratings r where r.userId = {userId}")] IEnumerable<Rating> ratings)
                
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteAsJsonAsync(ratings);

            return response;
        }
    }
}
