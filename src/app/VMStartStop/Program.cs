using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCo.TagManager.Application;
using MyCo.TagManager.Domain;
using MyCo.TagManager.Infrastrucutre;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddTransient<ITagManagerService, TagManagerService>();
        services.AddTransient<ITagsRepository, TagsRepository>();
        services.AddTransient<IClock, Clock>();
        services.AddAzureClients(builder =>
        {
            builder.UseCredential(new DefaultAzureCredential());
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
    })
    .Build();

host.Run();
