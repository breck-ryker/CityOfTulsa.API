using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Classes {

   public class PathSettings {

      public PathSettings() {
         //todo
      }

      public string APISwaggerURL { get; set; } = null;
      public string APILogInURL { get; set; } = null;
      public string APILogInPassword { get; set; } = null;
      public string APIAuthMethod { get; set; } = null;
      public string APIKey { get; set; } = null;

      public string TFDProblemsURL { get; set; } = null;
      public string TFDDivisionsURL { get; set; } = null;
      public string TFDStationsURL { get; set; } = null;
      public string TFDVehiclesURL { get; set; } = null;
      public string TFDVehicleIDsURL { get; set; } = null;
      public string TFDEventsURL { get; set; } = null;
      public string TFDEventCountURL { get; set; } = null;

      public string CityOfTulsaOrgURL { get; set; } = null;
   }
}
