using CityOfTulsaUI.Classes;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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

      private readonly ILogger<HomeController> _logger;
      private static readonly HttpClient _httpClient = new();
      private readonly IMemoryCache _cache;

      public string KeepSessionAlive() {

			return System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
      }

		public HomeController(
         ILogger<HomeController> logger,
         IMemoryCache memoryCache
      ) {
         _logger = logger;
         _cache = memoryCache;
      }

      public IActionResult Index() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel();
            HttpContext.Session.Set("UserModel", model);
         }

         return View(model);
      }

      public IActionResult TFDData() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel();
         }

         List<string> problems = null;

         if (!(_cache.TryGetValue(CacheKeys.COT_API_TFD_Problems.ToString(), out problems))) {

            var task = Task.Run(() => _httpClient.GetAsync("https://localhost:44305/tfd/problems"));
            task.Wait();
            var result = task.Result;

            if (result.IsSuccessStatusCode) {

               var readTask = result.Content.ReadAsStringAsync();
               readTask.Wait();

               problems = JsonConvert.DeserializeObject<List<string>>(readTask.Result);
            }
            else {
               ModelState.AddModelError(string.Empty, "Server error.");
            }

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

            // Save data in cache.
            _cache.Set(CacheKeys.COT_API_TFD_Problems.ToString(), problems, cacheEntryOptions);
         }

         this.ViewBag.Problems = problems;

         HttpContext.Session.Set("UserModel", model);

         return View(model);
      }

      public IActionResult AboutCOTWebAPI() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel();
            HttpContext.Session.Set("UserModel", model);
         }

         return View(model);
      }

      public IActionResult AboutCOTProject() {

         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");

         if (model == null) {
            model = new UserModel();
            HttpContext.Session.Set("UserModel", model);
         }

         return View(model);
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error() {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
