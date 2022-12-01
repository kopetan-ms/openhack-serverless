using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Messaging.ServiceBus;

var sbConnection = Environment.GetEnvironmentVariable("ServiceBusConnection");

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
       s.AddSingleton<ServiceBusClient>(new ServiceBusClient(sbConnection));
    })
    .Build();

await host.RunAsync();