
using Microsoft.Extensions.DependencyInjection;
using CityOfTulsaAPI.Classes;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JWTOrApiKeyRequiredAttribute : Attribute, IAuthorizationFilter {

   private AppSettings? _appSettings = null;

   public void OnAuthorization(AuthorizationFilterContext context) {

      if (context != null) {

         IServiceProvider services = context.HttpContext.RequestServices;
         _appSettings = services.GetService<AppSettings>();

         //get the authorization header
         Microsoft.Extensions.Primitives.StringValues authTokens;
         context.HttpContext.Request.Headers.TryGetValue("Authorization", out authTokens);

         var token = authTokens.FirstOrDefault();

         if (token != null) {

            string authToken = token;

            if (authToken != null) {

               if (IsValidToken(authToken)) {

                  context.HttpContext.Response.Headers.Add("Authorization", authToken);
                  context.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");

                  context.HttpContext.Response.Headers.Add("storeAccessiblity", "Authorized");

                  return;
               }
               else {
                  context.HttpContext.Response.Headers.Add("Authorization", authToken);
                  context.HttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");

                  context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                  context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";

                  context.Result = new JsonResult("NotAuthorized") {
                     Value = new {
                        Status = "Error",
                        Message = "Invalid Token"
                     },
                  };
               }

            }

         }
         else if (!(string.IsNullOrWhiteSpace(_appSettings?.HeaderAPIKeyName))) {
            //if the request header doesn't contain the authorization header, try to get the API-Key.
            Microsoft.Extensions.Primitives.StringValues apikey;
            var key = context.HttpContext.Request.Headers.TryGetValue(_appSettings.HeaderAPIKeyName, out apikey);
            var keyvalue = apikey.FirstOrDefault();

            //if the API-Key value is not null. validate the API-Key.
            if (keyvalue != null) {

               context.HttpContext.Response.Headers.Add("ApiKey", keyvalue);
               context.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");

               context.HttpContext.Response.Headers.Add("storeAccessiblity", "Authorized");

               return;
            }

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Please Provide authToken";

            context.Result = new JsonResult("Please Provide auth Token") {
               Value = new {
                  Status = "Error",
                  Message = "Please Provide auth Token"
               },
            };
         }
      }
   }

   public bool IsValidToken(string authToken) {

      //string jwtKey = _config["JWT:Key"];
      //string jwtIssuer = _config["JWT:Issuer"];

      if (string.IsNullOrWhiteSpace(_appSettings?.JWT?.Key)) {
         return false;
      }

      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT.Key));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      //validate Token here  
      return true;
   }
}