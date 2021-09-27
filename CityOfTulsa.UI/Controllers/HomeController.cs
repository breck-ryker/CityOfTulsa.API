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
using System.Threading.Tasks;
using static CityOfTulsaUI.Classes.CommonLib;

namespace CityOfTulsaUI.Controllers {

   public class HomeController : Controller {

      private readonly IConfiguration _config = null;
      private readonly ILogger<HomeController> _logger;
      private static readonly HttpClient _httpClient = new();
      private readonly IMemoryCache _cache;
      private readonly PathSettings _pathSettings;
      private readonly string _apiKey = null;

      public string KeepSessionAlive() {

			return System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
      }

		public HomeController(
         ILogger<HomeController> logger,
         IMemoryCache memoryCache,
         IConfiguration config,
         IOptions<PathSettings> pathSettings
      ) {
         _logger = logger;
         _cache = memoryCache;
         _config = config;
         _pathSettings = pathSettings.Value;

         _apiKey = _config.GetValue<string>(CONST_AppSettings_ApiKeyName);
      }

      public IActionResult Index() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel(_pathSettings);
            HttpContext.Session.Set("UserModel", model);
         }

         if (model.PathSettings == null) { model.PathSettings = _pathSettings; }

         return View(model);
      }

      public IActionResult AboutCOTWebAPI() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel(_pathSettings);
            HttpContext.Session.Set("UserModel", model);
         }

         if (model.PathSettings == null) { model.PathSettings = _pathSettings; }

         return View(model);
      }

      public IActionResult AboutCOTProject() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel(_pathSettings);
            HttpContext.Session.Set("UserModel", model);
         }

         if (model.PathSettings == null) { model.PathSettings = _pathSettings; }

         return View(model);
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error() {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
