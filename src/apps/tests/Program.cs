
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using tests.Contracts;

AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

var serviceProvider = CreateSerilogLogger(Configure(args));

var smokeTest = serviceProvider.GetRequiredService<ISmokeTest>();

try
{
    Task.WaitAny(smokeTest.Start());

    Environment.Exit(0);
}
catch
{
    Environment.Exit(-1);
}

static IServiceProvider CreateSerilogLogger(IServiceProvider provider)
{

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger();

    Log.Information($"Start Log..Host:{Environment.MachineName}  PID:{System.Diagnostics.Process.GetCurrentProcess().Id}");

    return provider;
}
static IServiceProvider Configure(string[] args)
{
    var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .AddEnvironmentVariables();


    var config = builder.Build();

    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(config);

    return services.BuildServiceProvider();
}

static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    Console.WriteLine($"Process crashing due to {e.ExceptionObject}");
}