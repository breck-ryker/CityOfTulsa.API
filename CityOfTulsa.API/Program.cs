using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsa.API {

   public class Program {

      public static void Main(string[] args) {

         CreateHostBuilder(args).Build().Run();

         //var host = new HostBuilder()
         //    .ConfigureAppConfiguration((hostContext, builder) => {
         //      // Other providers

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
              });
   }
}
