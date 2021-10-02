using CityOfTulsaAPI.Attributes;
using CityOfTulsaAPI.Classes;
using CityOfTulsaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityOfTulsaAPI.Controllers {

   [Route("[controller]")]
   [ApiController]
   public class TFDController : ControllerBase {

      private readonly DatabaseContext _dbcontext;
      private readonly IConfiguration _config;
      private readonly AppSettings _appSettings;

      public TFDController(
         DatabaseContext dbcontext,
         IConfiguration config,
         IOptions<AppSettings> appSettings
      ) {
         _dbcontext = dbcontext;
         _config = config;
         _appSettings = appSettings.Value;
      }

      [HttpGet]
      //[ApiKeyRequired]
      //[Authorize]
      [JWTOrApiKeyRequired]
      [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
      public IEnumerable<FireEventHelper> Get() {
         
         // just return the most recent n records
         return _dbcontext.FireEvents
            //.Where(e => e.ResponseDate >= DateTime.Now.AddDays(-7))
            .Include(e => e.FireVehicles)
            .Select(e => new FireEventHelper(e))
            .OrderByDescending(e => e.ResponseDate)
            .Take(_appSettings.EventCountMax)
            ;
      }

      // GET <TFDController>/5 -or- /TFD2021000053922
      [HttpGet("{id}")]
      //[ApiKeyRequired]
      //[Authorize]
      [JWTOrApiKeyRequired]
      [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
      // [Route("{id:alpha:minlength(1):maxlength(16)}")] // swagger hates this
      public FireEventHelper? Get(string id) {

         return _dbcontext.FireEvents
            .Where(e => e.FireEventID.ToString() == id || e.IncidentNumber == id)
            .Include(e => e.FireVehicles)
            .Select(e => new FireEventHelper(e))
            .FirstOrDefault()
            ;
      }

      [HttpGet("dates")]
      //[ApiKeyRequired]
      //[Authorize]
      [JWTOrApiKeyRequired]
      [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
      public IEnumerable<FireEventHelper> Get(
         string? mindate = null, 
         string? maxdate = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);

         return _dbcontext.FireEvents
            .Where(e => 
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin) 
               && 
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
            )
            .Include(e => e.FireVehicles)
            .Select(e => new FireEventHelper(e))
            .OrderBy(e => e.ResponseDate)
            .Take(_appSettings.EventCountMax)
            ;
      }

      [HttpGet("events")]
      //[ApiKeyRequired]
      //[Authorize]
      [JWTOrApiKeyRequired]
      [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { "*" })] 
      public IEnumerable<FireEventHelper> GetEvents(
         string? mindate = null,
         string? maxdate = null,
         string? problems = null,
         string? divisions = null,
         string? stations = null,
         string? vehicles = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);
         List<string> listProblems = (string.IsNullOrWhiteSpace(problems) ? new List<string>() : problems.Split(',').ToList());
         List<string> listDivisions = (string.IsNullOrWhiteSpace(divisions) ? new List<string>() : divisions.Split(',').ToList());
         List<string> listStations = (string.IsNullOrWhiteSpace(stations) ? new List<string>() : stations.Split(',').ToList());
         List<string> listVehicles = (string.IsNullOrWhiteSpace(vehicles) ? new List<string>() : vehicles.Split(',').ToList());

         return _dbcontext.FireEvents
            .Where(e =>
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin)
               &&
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
               &&
               (listProblems.Count == 0 || listProblems.Contains(e.Problem))
            )
            .Where(e =>
               e.FireVehicles
               .Where(v =>
                  (listDivisions.Count == 0 || listDivisions.Contains(v.Division))
                  &&
                  (listStations.Count == 0 || listStations.Contains(v.Station))
                  &&
                  (listVehicles.Count == 0 || listVehicles.Contains(v.FireVehicleID.ToString()) || listVehicles.Contains(v.VehicleID))
               ).Any()
            )
            .Select(e => new FireEventHelper(e))
            .Take(_appSettings.EventCountMax)
            ;
      }

      [HttpGet("eventcountmax")]
      //[ApiKeyRequired]
      //[Authorize]
      //[JWTOrApiKeyRequired]
      [ResponseCache(Duration = 720, Location = ResponseCacheLocation.Any)]
      public int GetEventCountMax() {
         return _appSettings.EventCountMax;
      }

      [HttpGet("eventcount")]
      //[ApiKeyRequired]
      //[Authorize]
      [JWTOrApiKeyRequired]
      [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { "*" })]
      public int GetEventCount(
         string? mindate = null,
         string? maxdate = null,
         string? problems = null,
         string? divisions = null,
         string? stations = null,
         string? vehicles = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);
         List<string> listProblems = (string.IsNullOrWhiteSpace(problems) ? new List<string>() : problems.Split(',').ToList());
         List<string> listDivisions = (string.IsNullOrWhiteSpace(divisions) ? new List<string>() : divisions.Split(',').ToList());
         List<string> listStations = (string.IsNullOrWhiteSpace(stations) ? new List<string>() : stations.Split(',').ToList());
         List<string> listVehicles = (string.IsNullOrWhiteSpace(vehicles) ? new List<string>() : vehicles.Split(',').ToList());

         return _dbcontext.FireEvents
            .Where(e =>
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin)
               &&
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
               &&
               (listProblems.Count == 0 || listProblems.Contains(e.Problem))
            )
            .Where(e =>
               e.FireVehicles
               .Where(v =>
                  (listDivisions.Count == 0 || listDivisions.Contains(v.Division))
                  &&
                  (listStations.Count == 0 || listStations.Contains(v.Station))
                  &&
                  (listVehicles.Count == 0 || listVehicles.Contains(v.FireVehicleID.ToString()) || listVehicles.Contains(v.VehicleID))
               ).Any()
            )
            .Select(e => new FireEventHelper(e))
            .Count()
            ;
      }

      [HttpGet("problems")]
      //[ApiKeyRequired]
      //[Authorize]
      //[JWTOrApiKeyRequired]
      [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
      public IEnumerable<string> GetProblems(
         string? mindate = null, 
         string? maxdate = null,
         string? divisions = null,
         string? stations = null,
         string? vehicles = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);
         List<string> listDivisions = (string.IsNullOrWhiteSpace(divisions) ? new List<string>() : divisions.Split(',').ToList());
         List<string> listStations = (string.IsNullOrWhiteSpace(stations) ? new List<string>() : stations.Split(',').ToList());
         List<string> listVehicles = (string.IsNullOrWhiteSpace(vehicles) ? new List<string>() : vehicles.Split(',').ToList());

         return _dbcontext.FireEvents
            .Where(e =>
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin)
               &&
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
            )
            .Include(e => e.FireVehicles)
            .Where(e =>
               e.FireVehicles
               .Where(v =>
                  (listDivisions.Count == 0 || listDivisions.Contains(v.Division))
                  &&
                  (listStations.Count == 0 || listStations.Contains(v.Station))
                  &&
                  (listVehicles.Count == 0 || listVehicles.Contains(v.FireVehicleID.ToString()) || listVehicles.Contains(v.VehicleID))
               ).Any()
            )
            .Select(e => e.Problem)
            .Where(p => !(string.IsNullOrWhiteSpace(p)))
            .OrderBy(p => p)
            .Distinct()
            .Take(1000)
            ;
      }

      [HttpGet("divisions")]
      //[ApiKeyRequired]
      //[Authorize]
      //[JWTOrApiKeyRequired]
      [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
      public IEnumerable<string> GetDivisions(
         string? mindate = null,
         string? maxdate = null,
         string? problems = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);
         List<string> listProblems = (string.IsNullOrWhiteSpace(problems) ? new List<string>() : problems.Split(',').ToList());

         return _dbcontext.FireEvents
            .Where(e =>
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin)
               &&
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
               &&
               (listProblems.Count == 0 || listProblems.Contains(e.Problem))
            )
            .Include(e => e.FireVehicles)
            .SelectMany(e => e.FireVehicles)
            .Select(v => v.Division)
            .Where(d => !(string.IsNullOrWhiteSpace(d)))
            .OrderBy(d => d)
            .Distinct()
            .Take(1000)
            ;
      }

      [HttpGet("stations")]
      //[ApiKeyRequired]
      //[Authorize]
      //[JWTOrApiKeyRequired]
      [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })] 
      public IEnumerable<string> GetStations(
         string? mindate = null,
         string? maxdate = null,
         string? problems = null,
         string? divisions = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);
         List<string> listDivisions = (string.IsNullOrWhiteSpace(divisions) ? new List<string>() : divisions.Split(',').ToList());
         List<string> listProblems = (string.IsNullOrWhiteSpace(problems) ? new List<string>() : problems.Split(',').ToList());

         return _dbcontext.FireEvents
            .Where(e =>
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin)
               &&
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
               &&
               (listProblems.Count == 0 || listProblems.Contains(e.Problem))
            )
            .Include(e => e.FireVehicles)
            .SelectMany(e => e.FireVehicles)
            .Where(v => (listDivisions.Count == 0 || listDivisions.Contains(v.Division)))
            .Select(v => v.Station)
            .Where(s => !(string.IsNullOrWhiteSpace(s)))
            .OrderBy(s => s)
            .Distinct()
            .Take(1000)
            ;
      }

      [HttpGet("vehicles")]
      //[ApiKeyRequired]
      //[Authorize]
      [JWTOrApiKeyRequired]
      [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
      public IEnumerable<FireVehicleHelper> GetVehicles(
         string? mindate = null,
         string? maxdate = null,
         string? fireeventids = null,
         string? incidentids = null,
         string? problems = null,
         string? divisions = null,
         string? stations = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);
         List<string> listFireEventIDs = (string.IsNullOrWhiteSpace(fireeventids) ? new List<string>() : fireeventids.Split(',').ToList());
         List<string> listIncidentIDs = (string.IsNullOrWhiteSpace(incidentids) ? new List<string>() : incidentids.Split(',').ToList());
         List<string> listDivisions = (string.IsNullOrWhiteSpace(divisions) ? new List<string>() : divisions.Split(',').ToList());
         List<string> listStations = (string.IsNullOrWhiteSpace(stations) ? new List<string>() : stations.Split(',').ToList());
         List<string> listProblems = (string.IsNullOrWhiteSpace(problems) ? new List<string>() : problems.Split(',').ToList());

         return _dbcontext.FireEvents
            .Where(e =>
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin)
               &&
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
               &&
               (listFireEventIDs.Count == 0 || listFireEventIDs.Contains(e.FireEventID.ToString()))
               &&
               (listIncidentIDs.Count == 0 || listIncidentIDs.Contains(e.IncidentNumber))
               &&
               (listProblems.Count == 0 || listProblems.Contains(e.Problem))
            )
            .Include(e => e.FireVehicles)
            .SelectMany(e => e.FireVehicles)
            .Where(v =>
               (listDivisions.Count == 0 || listDivisions.Contains(v.Division))
               &&
               (listStations.Count == 0 || listStations.Contains(v.Station))
            )
            .Select(v => new FireVehicleHelper(v))
            .Take(1000)
            ;
      }

      [HttpGet("vehicleids")]
      //[ApiKeyRequired]
      //[Authorize]
      //[JWTOrApiKeyRequired]
      [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })] 
      public IEnumerable<string> GetVehicleIDs(
         string? mindate = null,
         string? maxdate = null,
         string? fireeventids = null,
         string? incidentids = null,
         string? problems = null,
         string? divisions = null,
         string? stations = null
      ) {

         DateTime.TryParse(mindate, out DateTime dtMin);
         DateTime.TryParse(maxdate, out DateTime dtMax);
         List<string> listFireEventIDs = (string.IsNullOrWhiteSpace(fireeventids) ? new List<string>() : fireeventids.Split(',').ToList());
         List<string> listIncidentIDs = (string.IsNullOrWhiteSpace(incidentids) ? new List<string>() : incidentids.Split(',').ToList());
         List<string> listDivisions = (string.IsNullOrWhiteSpace(divisions) ? new List<string>() : divisions.Split(',').ToList());
         List<string> listStations = (string.IsNullOrWhiteSpace(stations) ? new List<string>() : stations.Split(',').ToList());
         List<string> listProblems = (string.IsNullOrWhiteSpace(problems) ? new List<string>() : problems.Split(',').ToList());

         return _dbcontext.FireEvents
            .Where(e =>
               (!(dtMin.IsValidValue()) || e.ResponseDate >= dtMin)
               &&
               (!(dtMax.IsValidValue()) || e.ResponseDate <= dtMax)
               &&
               (listFireEventIDs.Count == 0 || listFireEventIDs.Contains(e.FireEventID.ToString()))
               &&
               (listIncidentIDs.Count == 0 || listIncidentIDs.Contains(e.IncidentNumber))
               &&
               (listProblems.Count == 0 || listProblems.Contains(e.Problem))
            )
            .Include(e => e.FireVehicles)
            .SelectMany(e => e.FireVehicles)
            .Where(v => 
               (listDivisions.Count == 0 || listDivisions.Contains(v.Division)) 
               && 
               (listStations.Count == 0 || listStations.Contains(v.Station))
            )
            .Select(v => v.VehicleID)
            .Where(id => !(string.IsNullOrWhiteSpace(id)))
            .OrderBy(v => v)
            .Distinct()
            .Take(1000)
            ;
      }
   }
}
