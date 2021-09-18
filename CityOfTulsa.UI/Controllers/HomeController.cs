using CityOfTulsaUI.Classes;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Controllers {

   public class HomeController : Controller {

      private readonly ILogger<HomeController> _logger;
      private static readonly HttpClient _httpClient = new();

      public string KeepSessionAlive() {

			return System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
      }

		public HomeController(ILogger<HomeController> logger) {
         _logger = logger;
      }

      public IActionResult Index() {
         return View();
      }

      public IActionResult TFDData() {

         UserModel userModel = HttpContext.Session.Get<UserModel>("UserModel");

         if (userModel == null) {
            userModel = new UserModel();
         }

         var task = Task.Run(() => _httpClient.GetAsync("https://localhost:44305/tfd/problems"));
         task.Wait();
         var result = task.Result;

         if (result.IsSuccessStatusCode) {
            var readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();

            List<string> problems = JsonConvert.DeserializeObject<List<string>>(readTask.Result);
         }
         else //web api sent error response 
         {
            //log response status here.

            ModelState.AddModelError(string.Empty, "Server error.");
         }


         //_httpClient.DefaultRequestHeaders.Accept.Clear();
         //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
         //_httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

         //var problems = _httpClient.GetAsync("https://localhost:44305/tfd/problems");

         HttpContext.Session.Set("UserModel", userModel);

         return View(userModel);
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error() {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
