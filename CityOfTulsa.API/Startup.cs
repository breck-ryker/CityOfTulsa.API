using CityOfTulsaAPI.Classes;
using CityOfTulsaData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityOfTulsa.API {

   public class Startup {

      public IConfiguration Configuration { get; }
      public IWebHostEnvironment Environment { get; }

      public Startup(
         IConfiguration config,
         IWebHostEnvironment env
      ) {
         this.Configuration = config;
         this.Environment = env;
      }

      public void ConfigureServices(IServiceCollection services) {

         //https://stackoverflow.com/questions/57912012/net-core-3-upgrade-cors-and-jsoncycle-xmlhttprequest-error/58084628#58084628
         services.AddControllers().AddNewtonsoftJson(
            x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

         services.AddDbContext<DatabaseContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("CityOfTulsa"))
            );

         services.AddSwaggerGen(
            c => 
               {
                  c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CityOfTulsaData.com Web API", Version = "v1" });
               }
            );

         services.AddCors(a => {
            a.AddPolicy("AllowAll", builder =>
               builder.AllowAnyOrigin()
               .AllowAnyHeader()
            );
         });

         services.AddControllers();

         AppSettings appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
         services.AddSingleton(appSettings);
         services.Configure<AppSettings>(this.Configuration.GetSection("AppSettings"));

         services.AddAuthentication(x =>
         {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         })
         .AddJwtBearer(x =>
         {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters {
               ValidateIssuerSigningKey = true,
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidIssuer = appSettings.JWT.Issuer,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JWT.Key))
            };
         });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {

         if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
         }

         app.UseSwagger();
         app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "City of Tulsa Data: v1"));

         //app.UseHttpsRedirection();

         app.UseCors("AllowAll");

         app.UseRouting();

         app.UseResponseCaching();

         app.UseAuthentication();

         app.UseAuthorization();

         //loggerFactory.AddFile($@"{Directory.GetCurrentDirectory()}\Logs\cot-apilog-" + DateTime.Now.ToString("yyyyMMddDDD") + ".txt");

         app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
         });
      }
   }
}
