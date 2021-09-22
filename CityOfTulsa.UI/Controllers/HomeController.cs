using CityOfTulsaUI.Classes;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Mvc;
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

      public IActionResult TFDData() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel(_pathSettings);
         }

         List<string> problems = null;
         List<string> divisions = null;
         List<string> stations = null;
         List<string> vehicles = null;

         if (!(_cache.TryGetValue(CacheKeys.COT_API_TFD_Problems.ToString(), out problems))) {

            var task = Task.Run(() => _httpClient.GetAsync(_pathSettings.TFDProblemsURL));
            task.Wait();
            var result = task.Result;

            if (result.IsSuccessStatusCode) {

               var readTask = result.Content.ReadAsStringAsync();
               readTask.Wait();

               problems = JsonConvert.DeserializeObject<List<string>>(readTask.Result);
            }
            else {
               ModelState.AddModelError(string.Empty, "TFD Problem Data: Server Error");
            }

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

            // Save data in cache.
            _cache.Set(CacheKeys.COT_API_TFD_Problems.ToString(), problems, cacheEntryOptions);
         }

         if (!(_cache.TryGetValue(CacheKeys.COT_API_TFD_Divisions.ToString(), out divisions))) {

            var task = Task.Run(() => _httpClient.GetAsync(_pathSettings.TFDDivisionsURL));
            task.Wait();
            var result = task.Result;

            if (result.IsSuccessStatusCode) {

               var readTask = result.Content.ReadAsStringAsync();
               readTask.Wait();

               divisions = JsonConvert.DeserializeObject<List<string>>(readTask.Result);
            }
            else {
               ModelState.AddModelError(string.Empty, "TFD Division Data: Server Error");
            }

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

            // Save data in cache.
            _cache.Set(CacheKeys.COT_API_TFD_Divisions.ToString(), divisions, cacheEntryOptions);
         }

         if (!(_cache.TryGetValue(CacheKeys.COT_API_TFD_Stations.ToString(), out stations))) {

            var task = Task.Run(() => _httpClient.GetAsync(_pathSettings.TFDStationsURL));
            task.Wait();
            var result = task.Result;

            if (result.IsSuccessStatusCode) {

               var readTask = result.Content.ReadAsStringAsync();
               readTask.Wait();

               stations = JsonConvert.DeserializeObject<List<string>>(readTask.Result);
            }
            else {
               ModelState.AddModelError(string.Empty, "TFD Station Data: Server Error");
            }

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

            // Save data in cache.
            _cache.Set(CacheKeys.COT_API_TFD_Stations.ToString(), stations, cacheEntryOptions);
         }

         if (!(_cache.TryGetValue(CacheKeys.COT_API_TFD_Vehicles.ToString(), out vehicles))) {

            var task = Task.Run(() => _httpClient.GetAsync(_pathSettings.TFDVehiclesURL));
            task.Wait();
            var result = task.Result;

            if (result.IsSuccessStatusCode) {

               var readTask = result.Content.ReadAsStringAsync();
               readTask.Wait();

               vehicles = JsonConvert.DeserializeObject<List<string>>(readTask.Result);
            }
            else {
               ModelState.AddModelError(string.Empty, "TFD Vehicle Data: Server Error");
            }

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

            // Save data in cache.
            _cache.Set(CacheKeys.COT_API_TFD_Vehicles.ToString(), vehicles, cacheEntryOptions);
         }

         this.ViewBag.Problems = problems;
         this.ViewBag.Divisions = divisions;
         this.ViewBag.Stations = stations;
         this.ViewBag.Vehicles = vehicles;

         if (model.PathSettings == null) { model.PathSettings = _pathSettings; }

         HttpContext.Session.Set("UserModel", model);

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
