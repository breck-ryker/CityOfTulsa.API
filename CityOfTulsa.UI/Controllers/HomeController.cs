using CityOfTulsaData;
using CityOfTulsaUI.Classes;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static CityOfTulsaUI.Classes.CommonLib;

namespace CityOfTulsaUI.Controllers {

   public class HomeController : Controller {

      private readonly IConfiguration _config = null;
      private readonly ILogger<HomeController> _logger;
      private static readonly HttpClient _httpClient = new();
      private readonly IMemoryCache _cache;
      private readonly AppSettings _appSettings;

      public string KeepSessionAlive() {

			return System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
      }

		public HomeController(
         ILogger<HomeController> logger,
         IMemoryCache memoryCache,
         IConfiguration config,
         IOptions<AppSettings> appSettings
      ) {
         _logger = logger;
         _cache = memoryCache;
         _config = config;
         _appSettings = appSettings.Value;
      }

      public IActionResult Index() {

         UserModel model = this.VerifyUserModel();

         ////Type type = typeof(AppSettings);
         ////PropertyInfo[] properties = type.GetProperties();
         ////StringBuilder sb = new StringBuilder();

         ////foreach (PropertyInfo prop in properties) {
         ////   sb.Append(string.Format("<br/>" + "{0} = {1}", prop.Name, prop.GetValue(_appSettings, null)));
         ////}

         ////this.ViewBag.AppSettingsList = sb.ToString();

         return View(model);
      }

      public IActionResult AboutCOTWebAPI() {

         UserModel model = this.VerifyUserModel();

         this.ViewBag.AppSettings = _appSettings;

         return View(model);
      }

      public IActionResult AboutCOTProject() {

         UserModel model = this.VerifyUserModel();

         return View(model);
      }

      private UserModel VerifyUserModel() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel();
            HttpContext.Session.Set("UserModel", model);
         }

         return model;
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error() {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
