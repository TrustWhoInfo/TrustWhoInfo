using backend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.DefaultThreadCurrentCulture;

            try
            {
                var host = CreateHostBuilder(args).Build();
                var configuration = host.Services.GetService<IConfiguration>();
                var node = configuration.GetValue<string>("Node");

                var template = "{Timestamp:HH:mm:ss.fff} [{Level:u3}] ({ThreadId}:{UserName}) {Message}{NewLine}{Exception}";
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: template)
                    .WriteTo.File($"backend.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, outputTemplate: template)
                    .CreateLogger();

                AppDomain.CurrentDomain.UnhandledException += (s,e) => {
                    Exception ex = (Exception) e.ExceptionObject;
                    if (e.IsTerminating) {
                        Log.Fatal(ex, "Unhandled exception - application is terminating");
                        Log.CloseAndFlush();
                    } else {
                        Log.Fatal(ex, "Unhandled exception, but app is still on");
                    }                    
                };

                Log.Information("Starting application - " + node);
                Log.Information("\tArguments: " + string.Join(' ', args));
                host.Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.Information("Application is closing");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog();

                    webBuilder.ConfigureAppConfiguration((builder,options) =>
                    {
                        var path = Path.Combine(builder.HostingEnvironment.ContentRootPath, "../secrets/secrets.json");
                        options.AddJsonFile(path, optional:true);
                        path = Path.Combine(builder.HostingEnvironment.ContentRootPath, "../../secrets/secrets.json"); // 
                        options.AddJsonFile(path, optional: true);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
