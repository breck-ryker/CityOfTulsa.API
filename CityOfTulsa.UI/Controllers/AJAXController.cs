﻿using CityOfTulsaData;
using CityOfTulsaUI.Classes;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Controllers {

   public class AJAXController : Controller {

      private readonly IConfiguration _config = null;
      private readonly ILogger<HomeController> _logger;
      private static readonly HttpClient _httpClient = new();
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

         string url;
         Dictionary<string, string> dictQueryString;
         AJAXPayload payload = new AJAXPayload(msg);
         UserModel model = HttpContext.Session.Get<UserModel>("UserModel");
         
         if (model == null) {
            model = new UserModel(_pathSettings);
         }

         model.QuerySettings = HttpContext.Session.Get<QuerySettings>("UserModel.QuerySettings");
         model.PrevQuerySettings = HttpContext.Session.Get<QuerySettings>("UserModel.PrevQuerySettings");

         try {

            payload.timetext = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-fff");

            switch ((msg.cmd ?? "").Trim().ToLower()) {

               case "tfd.do-init-validation":

                  // may expand this branch in the future to do other kinds of server-side validation checks, not just for dates.
                  payload.returncode = CommonLib.validateDateFilters(model, ref payload).ToString();

                  break;

               case "tfd.date-changed":

                  if (!(string.IsNullOrWhiteSpace(msg.data)) && !(string.IsNullOrWhiteSpace(msg.context))) {

                     DateTime.TryParse(msg.data, out DateTime dt);

                     switch (model.QuerySettings.TFDDateFilterType) {
                        case DateFilterType.AfterDate:
                           model.QuerySettings.MaxDate = DateTime.MaxValue;
                           break;
                        case DateFilterType.BeforeDate:
                           msg.context = "maxdate";
                           model.QuerySettings.MinDate = DateTime.MinValue;
                           break;
                     }

                     switch ((msg.context ?? "").ToLower()) {

                        case "mindate":

                           model.QuerySettings.MinDateText = msg.data;

                           if (dt.IsValidValue()) {
                              model.QuerySettings.MinDate = dt;
                           }

                           break;

                        case "maxdate":

                           model.QuerySettings.MaxDateText = msg.data;

                           if (dt.IsValidValue()) {
                              model.QuerySettings.MaxDate = dt;
                           }

                           break;
                     }

                     if (model.QuerySettings.TFDDateFilterType == DateFilterType.OnDate) {
                        model.QuerySettings.MaxDate = model.QuerySettings.MinDate.AddDays(1).AddSeconds(-1);  // we just set mindate, maxdate should be set to the same day's max.
                     }

                     payload.returncode = CommonLib.validateDateFilters(model, ref payload).ToString();
                  }

                  break;

               case "tfd.show-datefilter-options":

                  model.QuerySettings.UseTFDDateFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.set-datefilter-option":

                  model.QuerySettings.TFDDateFilterType = (DateFilterType)msg.data.ToInteger();

                  payload.returncode = CommonLib.validateDateFilters(model, ref payload).ToString();

                  break;

               case "tfd.show-problemlist":

                  model.QuerySettings.UseTFDProblemFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.show-divisionlist":

                  model.QuerySettings.UseTFDDivisionFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.show-stationlist":

                  model.QuerySettings.UseTFDStationFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.show-vehiclelist":

                  model.QuerySettings.UseTFDVehicleFilter = (msg.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.select-problem":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.QuerySettings.TFDProblems.Contains(msg.context))) {
                           model.QuerySettings.TFDProblems.Add(msg.context);
                        }
                     }
                     else {
                        if (model.QuerySettings.TFDProblems.Contains(msg.context)) {
                           model.QuerySettings.TFDProblems.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.select-division":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.QuerySettings.TFDDivsions.Contains(msg.context))) {
                           model.QuerySettings.TFDDivsions.Add(msg.context);
                        }
                     }
                     else {
                        if (model.QuerySettings.TFDDivsions.Contains(msg.context)) {
                           model.QuerySettings.TFDDivsions.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.select-station":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.QuerySettings.TFDStations.Contains(msg.context))) {
                           model.QuerySettings.TFDStations.Add(msg.context);
                        }
                     }
                     else {
                        if (model.QuerySettings.TFDStations.Contains(msg.context)) {
                           model.QuerySettings.TFDStations.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.select-vehicle":

                  if (!(string.IsNullOrWhiteSpace(msg.context))) {
                     if (msg.data.ToInteger() > 0) {
                        if (!(model.QuerySettings.TFDVehicles.Contains(msg.context))) {
                           model.QuerySettings.TFDVehicles.Add(msg.context);
                        }
                     }
                     else {
                        if (model.QuerySettings.TFDVehicles.Contains(msg.context)) {
                           model.QuerySettings.TFDVehicles.Remove(msg.context);
                        }
                     }
                  }

                  break;

               case "tfd.multi-select-problems":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDProblems = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDProblems.Clear();
                     }
                  }

                  break;

               case "tfd.multi-select-divisions":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDDivsions = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDDivsions.Clear();
                     }
                  }

                  break;

               case "tfd.multi-select-stations":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDStations = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDStations.Clear();
                     }
                  }

                  break;

               case "tfd.multi-select-vehicles":

                  if (!(string.IsNullOrWhiteSpace(msg.context)) && msg.values != null && msg.values.Length > 0) {
                     if (msg.context.Equals("select-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDVehicles = msg.values.ToList();
                     }
                     else if (msg.context.Equals("unselect-all", StringComparison.OrdinalIgnoreCase)) {
                        model.QuerySettings.TFDVehicles.Clear();
                     }
                  }

                  break;

               case "tfd.run-search":

                  DateTime minDate = (model.QuerySettings.UseTFDDateFilter ? model.QuerySettings.MinDate : DateTime.MinValue);
                  DateTime maxDate = (model.QuerySettings.UseTFDDateFilter ? model.QuerySettings.MaxDate : DateTime.MinValue);
                  string problems = (model.QuerySettings.UseTFDProblemFilter && model.QuerySettings.TFDProblems.Count > 0 ? string.Join(",", model.QuerySettings.TFDProblems) : null);
                  string divisions = (model.QuerySettings.UseTFDDivisionFilter && model.QuerySettings.TFDDivsions.Count > 0 ? string.Join(",", model.QuerySettings.TFDDivsions) : null);
                  string stations = (model.QuerySettings.UseTFDStationFilter && model.QuerySettings.TFDStations.Count > 0 ? string.Join(",", model.QuerySettings.TFDStations) : null);
                  string vehicleids = (model.QuerySettings.UseTFDVehicleFilter && model.QuerySettings.TFDVehicles.Count > 0 ? string.Join(",", model.QuerySettings.TFDVehicles) : null);

                  dictQueryString = new Dictionary<string, string>() 
                  {
                     { "mindate", minDate.ToString("MM/dd/yyyy HH:mm") },
                     { "maxdate", maxDate.ToString("MM/dd/yyyy HH:mm") },
                     { "problems", problems },
                     { "divisions", divisions },
                     { "stations", stations },
                     { "vehicles", vehicleids }
                  };

                  model.ExecuteQuery();

                  url = QueryHelpers.AddQueryString(_pathSettings.TFDEventCountURL, dictQueryString);

                  var eventTask = Task.Run(() => _httpClient.GetAsync(url));
                  eventTask.Wait();
                  var result = eventTask.Result;

                  if (result.IsSuccessStatusCode) {

                     var readTask = result.Content.ReadAsStringAsync();
                     readTask.Wait();

                     model.TFDEventsCountResult = JsonConvert.DeserializeObject<int>(readTask.Result);
                     payload.returncode = model.TFDEventsCountResult.ToString();
                  }
                  else {
                     ModelState.AddModelError(string.Empty, "TFD Station Data: Server Error");
                     payload.returncode = (-1).ToString();
                  }

                  break;

               case "tfd-results.get-event-vehicles":

                  dictQueryString = new Dictionary<string, string>()
                  {
                     { "incidentids", msg.ids[0] }
                  };

                  url = QueryHelpers.AddQueryString(_pathSettings.TFDVehiclesURL, dictQueryString);

                  var vehicleTask = Task.Run(() => _httpClient.GetAsync(url));
                  vehicleTask.Wait();
                  var vehicleResult = vehicleTask.Result;

                  if (vehicleResult.IsSuccessStatusCode) {

                     var readTask = vehicleResult.Content.ReadAsStringAsync();
                     readTask.Wait();

                     payload.markup = readTask.Result;

                     //IEnumerable<FireVehicleHelper> vehicles = JsonConvert.DeserializeObject<IEnumerable<FireVehicleHelper>>(readTask.Result);
                  }
                  else {
                     ModelState.AddModelError(string.Empty, "TFD Station Data: Server Error");
                  }

                  break;
            }

            if (model.PrevQuerySettings == null) {
               payload.dict["settings-has-changes"] = "y";
            }
            else { 
               Comparer comparer = new();
               IEnumerable<Difference> diffs;
               bool isEqual = comparer.Compare(model.QuerySettings, model.PrevQuerySettings, out diffs);
               payload.dict["search-has-changes"] = (isEqual ? "n" : "y");
            }

            payload.dict["tfd-search-results-count"] = model.TFDEventsCountResult.ToString();
         }
         catch (System.Exception ex) {

            payload.cmd = "error";
            payload.origcmd = msg.cmd;
            payload.value = (-1).ToString();
            payload.msg = ex.ToString();
         }

         HttpContext.Session.Set("UserModel", model);
         HttpContext.Session.Set("UserModel.QuerySettings", model.QuerySettings);
         HttpContext.Session.Set("UserModel.PrevQuerySettings", model.PrevQuerySettings);

         JsonResult jsonResult = new(
            JsonConvert.SerializeObject(payload)
            );

         return jsonResult;
      }
   }
}
