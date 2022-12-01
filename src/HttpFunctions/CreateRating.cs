using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Openhack.MS
{
    public class CreateRating
    {
        private readonly ILogger _logger;
        private static HttpClient httpClient = new HttpClient();

        public CreateRating(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CreateRating>();
        }

        [Function("CreateRating")]
        public async Task<MultiResponse> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //get the json payload
            var request = new StreamReader(req.Body).ReadToEnd();
            var rating = JsonSerializer.Deserialize<Rating>(request);

            _logger.LogInformation($"Data userid {rating.userId}, product id {rating.productId}");

            //validate json data with provided APIs
            
            var urlProduct = $"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={rating.productId}";
            using (HttpResponseMessage productResponse = httpClient.GetAsync(urlProduct).Result)
            {
                if (productResponse.StatusCode != HttpStatusCode.OK)
                {
                    return new MultiResponse()
                    {
                        rating = null,
                        HttpResponse = req.CreateResponse(HttpStatusCode.NotFound)
                    };
                }
            }

            var urlUser = $"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={rating.userId}";
            using (HttpResponseMessage userResponse = httpClient.GetAsync(urlUser).Result)
            {
                if (userResponse.StatusCode != HttpStatusCode.OK)
                {
                    return new MultiResponse()
                    {
                        rating = null,
                        HttpResponse = req.CreateResponse(HttpStatusCode.NotFound)
                    };
                }
            }

            //update the json with a timestamp and id 
            rating.id = Guid.NewGuid();
            rating.timestamp = DateTime.Now;

            //save the json to cosmosDB
            var response = req.CreateResponse(HttpStatusCode.OK);
            
            await response.WriteAsJsonAsync(rating);

            // Return a response to both HTTP trigger and Azure Cosmos DB output binding.
            return new MultiResponse()
            {
                rating = rating,
                HttpResponse = response
            };
        }
    }

    public class MultiResponse
    {
        [CosmosDBOutput(Consts.CosmosDBDatabase, Consts.CosmosDBCollection,
            ConnectionStringSetting = Consts.CosmosConnectionStringSetting, CreateIfNotExists = true)]
        public Rating rating { get; set; }
        public HttpResponseData HttpResponse { get; set; }
    }
    public class Rating
    {
        public Guid id {get; set;}
        public Guid userId {get; set;}
        public Guid productId {get; set;}
        public string locationName {get; set;}
        public int rating {get; set;}
        public string userNote {get; set;}
        public DateTime timestamp {get; set;}
    }
}

