using CityOfTulsaAPI.Attributes;
using CityOfTulsaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

      public TFDController(DatabaseContext dbcontext) {
         _dbcontext = dbcontext;
      }

      [HttpGet]
      public IEnumerable<FireEventHelper> Get() {
         
         // just return the most recent n records
         return _dbcontext.FireEvents
            //.Where(e => e.ResponseDate >= DateTime.Now.AddDays(-7))
            .Include(e => e.FireVehicles)
            .Select(e => new FireEventHelper(e))
            .OrderByDescending(e => e.ResponseDate)
            .Take(1000)
            ;
      }

      // GET <TFDController>/5 -or- /TFD2021000053922
      [HttpGet("{id}")]
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
            .Take(1000)
            ;
      }

      [HttpGet("events")]
      [ApiKeyRequired]
      //[Authorize]
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
            .Take(1000)
            ;
      }

      [HttpGet("eventcount")]
      [ApiKeyRequired]
      //[Authorize]
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
            //.OrderBy(e => e.ResponseDate)
            .Count()
            ;
      }

      [HttpGet("problems")]
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
      [ApiKeyRequired]
      //[Authorize]
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
