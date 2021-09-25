using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Classes {

   public class PathSettings {

      public PathSettings() {
         //todo
      }

      public string WebAPISwaggerURL { get; set; } = null;

      public string TFDProblemsURL { get; set; } = null;
      public string TFDDivisionsURL { get; set; } = null;
      public string TFDStationsURL { get; set; } = null;
      public string TFDVehiclesURL { get; set; } = null;
      public string TFDEventsURL { get; set; } = null;
      public string TFDEventCountURL { get; set; } = null;

      public string CityOfTulsaOrgURL { get; set; } = null;
   }
}
