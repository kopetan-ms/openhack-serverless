using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

var sbConnection = Environment.GetEnvironmentVariable("ServiceBusConnection");
var storageConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnection");
var storageContainerName = Openhack.MS.Consts.BlobStoragePath;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
       s.AddSingleton<ServiceBusClient>(new ServiceBusClient(sbConnection));
       s.AddSingleton<BlobContainerClient>( (x) => {
            var container = new BlobContainerClient(storageConnectionString, storageContainerName);
            container.Create();
            return container;
       } );
    })
    .Build();

await host.RunAsync();