using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCo.TagManager;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddTransient<ITagManagerService, TagManagerService>();
        services.AddTransient<ITagsRepository, TagsRepository>();
        services.AddAzureClients(builder =>
        {
            builder.UseCredential(new DefaultAzureCredential());
        });
    })
    .Build();

host.Run();
