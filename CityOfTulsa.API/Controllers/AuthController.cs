

using CityOfTulsaAPI.Classes;
using CityOfTulsaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityOfTulsaAPI.Controllers {

   [Route("[controller]")]
   [ApiController]
   public class AuthController : ControllerBase {

      private IConfiguration _config;
      private readonly AppSettings _appSettings;

      public AuthController(
         IConfiguration config,
         IOptions<AppSettings> appSettings
      ) {

         _config = config;
         _appSettings = appSettings.Value;
      }

      [AllowAnonymous]
      [HttpPost("login")]
      public IActionResult LogIn([FromBody] UserAuthInfo login) {

         IActionResult response = Unauthorized();
         var user = AuthenticateUser(login);

         if (user != null) {
            var tokenString = GenerateJSONWebToken(user);
            response = Ok(new { token = tokenString });
         }

         return response;
      }

      private string GenerateJSONWebToken(UserAuthInfo userInfo) {

         var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT.Key));
         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

         var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, (userInfo.UserName ?? "")),
            new Claim(JwtRegisteredClaimNames.Email, (userInfo.EmailAddress ?? "")),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
         };

         var token = new JwtSecurityToken(
            _appSettings.JWT.Issuer,
            _appSettings.JWT.Audience,
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
            );

         return new JwtSecurityTokenHandler().WriteToken(token);
      }

      private UserAuthInfo? AuthenticateUser(UserAuthInfo login) {

         //Validate the User Credentials       
         if ((_appSettings.GeneralPassword ?? "").Equals((login.Password ?? ""))) {
            return login;
         }

         return null;
      }
   }
}