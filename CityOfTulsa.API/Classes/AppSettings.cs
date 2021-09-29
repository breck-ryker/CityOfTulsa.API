using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsaAPI.Classes {

   public class AppSettings {

      public string APIKey { get; set; } = string.Empty;
      public string HeaderAPIKeyName { get; set; } = string.Empty;
      public string GeneralPassword { get; set; } = string.Empty;

      public JWT JWT { get; set; } = new JWT();
   }

   public class JWT {

      public string Key { get; set; } = string.Empty;
      public string Issuer { get; set; } = string.Empty;
      public string Audience { get; set; } = string.Empty;
   }
}
