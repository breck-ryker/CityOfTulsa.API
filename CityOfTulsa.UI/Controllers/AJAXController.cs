using CityOfTulsaUI.Classes;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Controllers {

   public class AJAXController : Controller {

      private readonly IConfiguration _config = null;
      private readonly ILogger<HomeController> _logger;
      //private static readonly HttpClient _httpClient = new();
      private readonly IMemoryCache _cache;
      private readonly PathSettings _pathSettings;

      public AJAXController(
         ILogger<HomeController> logger,
         IMemoryCache memoryCache,
         IConfiguration config,
         IOptions< PathSettings > pathSettings
      ) {
         _logger = logger;
         _cache = memoryCache;
         _config = config;
         _pathSettings = pathSettings.Value;
      }

      [HttpPost]
      public IActionResult ProcessMessage(
         [FromBody]AJAXMessage msg
      ) {

         AJAXPayload payload = new AJAXPayload(msg);
         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");
         
         if (model == null) {
            model = new UserModel(_pathSettings);
         }

         try {

            payload.timetext = System.DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-fff");

            switch ((msg.cmd ?? "").Trim().ToLower()) {

               case "tfd.do-init-validation":

                  // may expand this block in the future to do other kinds of server-side validation checks, not just for dates.
                  payload.returncode = CommonLib.validateDateFilters(model, ref payload).ToString();

                  break;

               case "tfd.date-changed":

                  if (!(string.IsNullOrWhiteSpace(msg.data)) && !(string.IsNullOrWhiteSpace(msg.context))) {

                     DateTime.TryParse(msg.data, out DateTime dt);

                     switch ((msg.context ?? "").ToLower()) {

                        case "mindate":

                           model.MinDateText = msg.data;

                           if (dt.IsValidValue()) {
                              model.MinDate = dt;
                           }

                           break;

                        case "maxdate":

                           model.MaxDateText = msg.data;

                           if (dt.IsValidValue()) {
                              model.MaxDate = dt;
                           }

                           break;
                     }

                     payload.returncode = CommonLib.validateDateFilters(model, ref payload).ToString();
                  }

                  break;

               case "tfd.show-datefilter-options":

                  model.UseTFDDateFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.set-datefilter-option":

                  model.TFDDateFilterType = (DateFilterType)msg.data.ToInteger();

                  payload.returncode = CommonLib.validateDateFilters(model, ref payload).ToString();

                  break;

               case "tfd.show-problemlist":

                  model.UseTFDProblemFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.show-divisionlist":

                  model.UseTFDDivisionFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.show-stationlist":

                  model.UseTFDStationFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.show-vehiclelist":

                  model.UseTFDVehicleFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.select-problem":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.TFDProblems.Contains(msg.context))) {
                           model.TFDProblems.Add(msg.context);
                        }
                     }
                     else {
                        if (model.TFDProblems.Contains(msg.context)) {
                           model.TFDProblems.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.select-division":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.TFDDivsions.Contains(msg.context))) {
                           model.TFDDivsions.Add(msg.context);
                        }
                     }
                     else {
                        if (model.TFDDivsions.Contains(msg.context)) {
                           model.TFDDivsions.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.select-station":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.TFDStations.Contains(msg.context))) {
                           model.TFDStations.Add(msg.context);
                        }
                     }
                     else {
                        if (model.TFDStations.Contains(msg.context)) {
                           model.TFDStations.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.select-vehicle":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.TFDVehicles.Contains(msg.context))) {
                           model.TFDVehicles.Add(msg.context);
                        }
                     }
                     else {
                        if (model.TFDVehicles.Contains(msg.context)) {
                           model.TFDVehicles.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.multi-select-problems":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDProblems = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDProblems.Clear();
                     }
                  }

                  break;

               case "tfd.multi-select-divisions":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDDivsions = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDDivsions.Clear();
                     }
                  }

                  break;

               case "tfd.multi-select-stations":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDStations = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDStations.Clear();
                     }
                  }

                  break;

               case "tfd.multi-select-vehicles":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDVehicles = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.TFDVehicles.Clear();
                     }
                  }

                  break;
            }
         }
         catch (System.Exception ex) {

            payload.cmd = "error";
            payload.origcmd = msg.cmd;
            payload.value = (-1).ToString();
            payload.msg = ex.ToString();
         }

         HttpContext.Session.Set("UserModel", model);

         JsonResult jsonResult = new(
            JsonConvert.SerializeObject(payload)
            );

         return jsonResult;
      }
   }
}
