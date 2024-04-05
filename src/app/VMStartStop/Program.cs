using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCo.TagManager;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddTransient<ITagManagerService, TagManagerService>();
    })
    .Build();

host.Run();
