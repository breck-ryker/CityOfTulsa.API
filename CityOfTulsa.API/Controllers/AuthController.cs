

using CityOfTulsaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
      //private readonly string? _generalPassword = null;
      //private const string CONST_AppSettings_GeneralPasswordName = "GeneralPassword";

      public AuthController(IConfiguration config) {

         _config = config;

         //_generalPassword = _config.GetValue<string>(CONST_AppSettings_GeneralPasswordName);
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

         string jwtKey = _config["AppSettings:JWT:Key"];
         string jwtIssuer = _config["AppSettings:JWT:Issuer"];

         var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

         var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, (userInfo.UserName ?? "")),
            new Claim(JwtRegisteredClaimNames.Email, (userInfo.EmailAddress ?? "")),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
         };

         var token = new JwtSecurityToken(
            jwtIssuer,
            jwtIssuer,
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
            );

         return new JwtSecurityTokenHandler().WriteToken(token);
      }

      private UserAuthInfo? AuthenticateUser(UserAuthInfo login) {

         string _generalPassword = _config["AppSettings:JWT:Key"];

         //Validate the User Credentials       
         if ((_generalPassword ?? "").Equals((login.Password ?? ""))) {
            return login;
         }

         return null;
      }
   }
}