using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CityOfTulsaAPI.Middleware {

   public class ApiKeyMiddleware {

      private readonly RequestDelegate _next;
      private const string CONST_AppSettings_ApiKeyName = "ApiKey";
      private const string CONST_Headers_ApiKeyName = "ApiKey";

      public ApiKeyMiddleware(RequestDelegate next) {
         _next = next;
      }

      public async Task InvokeAsync(HttpContext context) {

         if (!(context.Request.Headers.TryGetValue(CONST_Headers_ApiKeyName, out var extractedApiKey))) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api Key was not provided. (Using ApiKeyMiddleware) ");
            return;
         }

         var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

         var apiKey = appSettings.GetValue<string>(CONST_AppSettings_ApiKeyName);

         if (!(apiKey.Equals(extractedApiKey))) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client. (Using ApiKeyMiddleware)");
            return;
         }

         await _next(context);
      }
   }
}