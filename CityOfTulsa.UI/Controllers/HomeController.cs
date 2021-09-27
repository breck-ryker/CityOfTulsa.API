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

      public IActionResult TFDSearch() {

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

         if (!(_cache.TryGetValue(CacheKeys.COT_API_TFD_VehicleIDs.ToString(), out vehicles))) {

            var task = Task.Run(() => _httpClient.GetAsync(_pathSettings.TFDVehicleIDsURL));
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
            _cache.Set(CacheKeys.COT_API_TFD_VehicleIDs.ToString(), vehicles, cacheEntryOptions);
         }

         this.ViewBag.Problems = problems;
         this.ViewBag.Divisions = divisions;
         this.ViewBag.Stations = stations;
         this.ViewBag.Vehicles = vehicles;

         if (model.PathSettings == null) { model.PathSettings = _pathSettings; }

         HttpContext.Session.Set("UserModel", model);

         return View(model);
      }

      public IActionResult TFDResults() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel(_pathSettings);
         }

         model.QuerySettings = HttpContext.Session.Get<QuerySettings>("UserModel.QuerySettings");
         model.PrevQuerySettings = HttpContext.Session.Get<QuerySettings>("UserModel.PrevQuerySettings");

         DateTime minDate = (model.QuerySettings.UseTFDDateFilter ? model.QuerySettings.MinDate : DateTime.MinValue);
         DateTime maxDate = (model.QuerySettings.UseTFDDateFilter ? model.QuerySettings.MaxDate : DateTime.MinValue);
         string problems = (model.QuerySettings.UseTFDProblemFilter && model.QuerySettings.TFDProblems.Count > 0 ? string.Join(",", model.QuerySettings.TFDProblems) : null);
         string divisions = (model.QuerySettings.UseTFDDivisionFilter && model.QuerySettings.TFDDivsions.Count > 0 ? string.Join(",", model.QuerySettings.TFDDivsions) : null);
         string stations = (model.QuerySettings.UseTFDStationFilter && model.QuerySettings.TFDStations.Count > 0 ? string.Join(",", model.QuerySettings.TFDStations) : null);
         string vehicles = (model.QuerySettings.UseTFDVehicleFilter && model.QuerySettings.TFDVehicles.Count > 0 ? string.Join(",", model.QuerySettings.TFDVehicles) : null);

         var qryString = new Dictionary<string, string>()
         {
            { "mindate", minDate.ToString("MM/dd/yyyy HH:mm") },
            { "maxdate", maxDate.ToString("MM/dd/yyyy HH:mm") },
            { "problems", problems },
            { "divisions", divisions },
            { "stations", stations },
            { "vehicles", vehicles }
         };

         string url = QueryHelpers.AddQueryString(_pathSettings.TFDEventsURL, qryString);

         var task = Task.Run(() => _httpClient.GetAsync(url));
         task.Wait();
         var result = task.Result;

         if (result.IsSuccessStatusCode) {

            var readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();

            IEnumerable<FireEventHelper> events = JsonConvert.DeserializeObject<IEnumerable<FireEventHelper>>(readTask.Result);

            ViewBag.FireEvents = events;
         }
         else {
            ModelState.AddModelError(string.Empty, "TFD Station Data: Server Error");

            ViewBag.FireEvents = null;
         }

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
