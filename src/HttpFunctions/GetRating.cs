using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Openhack.MS
{
    public class GetRating
    {
        private readonly ILogger _logger;

        public GetRating(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetRating>();
        }

        [Function("GetRating")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get",Route = "GetRating/{ratingId}")] HttpRequestData req, 
        [CosmosDBInput(databaseName: Consts.CosmosDBDatabase,
                       collectionName: Consts.CosmosDBCollection,
                       ConnectionStringSetting = Consts.ConnectionStringSetting,
                       SqlQuery = "select * from Ratings r where r.id = {ratingId}")] IEnumerable<Rating> ratings)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(ratings.Single());

            return response;
        }
    }
}
