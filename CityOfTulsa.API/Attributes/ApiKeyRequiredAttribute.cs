
using CityOfTulsaAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
 
namespace CityOfTulsaAPI.Attributes {

   [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
   public class ApiKeyRequiredAttribute : Attribute, IAsyncActionFilter {

      private AppSettings? _appSettings = null;

      public async Task OnActionExecutionAsync(
         ActionExecutingContext context, 
         ActionExecutionDelegate next
      ) {

         Microsoft.Extensions.Primitives.StringValues headerApiKey = new Microsoft.Extensions.Primitives.StringValues();
         IServiceProvider services = context.HttpContext.RequestServices;
         _appSettings = services.GetService<AppSettings>();

         if (
            _appSettings != null 
            && 
            !(string.IsNullOrWhiteSpace(_appSettings.HeaderAPIKeyName)) 
            && 
            !(context.HttpContext.Request.Headers.TryGetValue(_appSettings.HeaderAPIKeyName, out headerApiKey))
         ) {

            context.Result = new ContentResult() {
               StatusCode = 401,
               Content = "Api Key not found"
            };

            return;
         }

         if (_appSettings != null && !(_appSettings.APIKey.Equals(headerApiKey))) {

            context.Result = new ContentResult() {
               StatusCode = 401,
               Content = "Api Key not valid"
            };

            return;
         }

         await next();
      }
   }
}