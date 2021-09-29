
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
using System.IdentityModel.Tokens.Jwt;

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

         var authToken = authTokens.FirstOrDefault();

         if (authToken != null) {

            if (IsValidToken(authToken)) {

               context.HttpContext.Response.Headers.Add("Authorization", authToken);
               context.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");

               context.HttpContext.Response.Headers.Add("storeAccessiblity", "Authorized");

               return;
            }
            else {
               context.HttpContext.Response.Headers.Add("Authorization", authToken);
               context.HttpContext.Response.Headers.Add("AuthStatus", "Not Authorized");

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
         else if (!(string.IsNullOrWhiteSpace(_appSettings?.HeaderAPIKeyName))) {

            //if the request header doesn't contain the authorization header, try to get the API Key.
            Microsoft.Extensions.Primitives.StringValues values;
            var gotVal = context.HttpContext.Request.Headers.TryGetValue(_appSettings.HeaderAPIKeyName, out values);
            var keyValue = values.FirstOrDefault();

            //if the API Key value is not null. validate the API-Key.
            if ((keyValue ?? "").Equals(_appSettings.APIKey)) {

               context.HttpContext.Response.Headers.Add(_appSettings.HeaderAPIKeyName, keyValue);
               context.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");

               return;
            }

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "API Key is invalid";

            context.Result = new JsonResult("API Key is invalid") {
               Value = new {
                  Status = "Error",
                  Message = "API Key is invalid"
               },
            };
         }
      }
   }

   public bool IsValidToken(string authToken) {

      if (string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(_appSettings?.JWT?.Key)) {
         return false;
      }

      if (authToken.IndexOf(' ') >= 0) {
         var parts = authToken.Split(' ');
         authToken = parts[parts.Length - 1];
      }

      var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.JWT.Key));

      var tokenHandler = new JwtSecurityTokenHandler();

      try {
         tokenHandler.ValidateToken(
            authToken, 
            new TokenValidationParameters {
               ValidateIssuerSigningKey = true,
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidIssuer = _appSettings.JWT.Issuer,
               //ValidAudience = ,
               IssuerSigningKey = securityKey
            }, 
            out SecurityToken validatedToken
         );
      }
      catch {
         return false;
      }

      return true;
   }
}