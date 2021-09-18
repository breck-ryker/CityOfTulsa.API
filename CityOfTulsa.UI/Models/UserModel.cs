using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Models {

   public enum DateFilterType {
      None = 0,
      OnDate = 1,
      AfterDate = 2,
      BeforeDate = 3,
      BetweenDates = 4
   }

   public class UserModel {

      public bool UseTFDDateFilter { get; set; } = false;
      public DateFilterType TFDDateFilterType { get; set; } = DateFilterType.OnDate;

      public bool UseTFDProblemFilter { get; set; } = false;
      public List<string> TFDProblems { get; set; }
   }
}
