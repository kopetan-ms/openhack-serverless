using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Openhack.MS
{
    public class CreateRating
    {
        private readonly ILogger _logger;

        public CreateRating(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CreateRating>();
        }

        [Function("CreateRating")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //get the json payload
            var request = new StreamReader(req.Body).ReadToEnd();
            var rating = JsonConvert.DeserializeObject<Rating>(request);

            _logger.LogInformation($"Data userid {rating.userId}, product id {rating.productId}");

            //validate json data with provided APIs
            using (HttpClient client = new HttpClient())
            {
                var urlProduct = $"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={rating.productId}";
                using (HttpResponseMessage productResponse = client.GetAsync(urlProduct).Result)
                {
                    if (productResponse.StatusCode != HttpStatusCode.OK)
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }

                var urlUser = $"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={rating.userId}";
                using (HttpResponseMessage userResponse = client.GetAsync(urlUser).Result)
                {
                    if (userResponse.StatusCode != HttpStatusCode.OK)
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
            }

            //update the json with a timestamp and id 
            rating.id = new Guid();
            rating.timestamp = DateTime.Now;

            //save the json to cosmosDB


            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
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
