using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsa.API {

   public class Program {

      public static void Main(string[] args) {

         Log.Logger = new LoggerConfiguration()
             .WriteTo.File(
                 path: "Logs\\autolog-.txt",
                 outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                 rollingInterval: RollingInterval.Day,
                 restrictedToMinimumLevel: LogEventLevel.Information
             ).CreateLogger();

         try {
            Log.Information("Application is starting");
            CreateHostBuilder(args).Build().Run();
         }
         catch (Exception ex) {
            Log.Fatal(ex, "Application failed to start");
         }
         finally {
            Log.CloseAndFlush();
         }

         //CreateHostBuilder(args).Build().Run();

         //var host = new HostBuilder()
         //    .ConfigureAppConfiguration((hostContext, builder) => {

         //      if (hostContext.HostingEnvironment.IsDevelopment()) {
         //          builder.AddUserSecrets<Program>();
         //       }
         //    })
         //    .Build();

         //host.Run();

         //----------------------------------------------------------------------

         //var builder = new ConfigurationBuilder()
         //    .SetBasePath(env.ContentRootPath)
         //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
         //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

         //if (env.IsDevelopment()) {
         //   builder.AddUserSecrets();
         //}

         //builder.AddEnvironmentVariables();
         //Configuration = builder.Build();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
               webBuilder.UseStartup<Startup>();
            })
            .UseSerilog()
         ;
            //.UseSerilog((hostingContext, loggerConfig) =>
            //   loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
            //);
   }
}
