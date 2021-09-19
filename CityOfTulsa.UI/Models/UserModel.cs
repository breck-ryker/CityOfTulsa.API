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

      private List<string> _problems = null;
      private List<string> _divisions = null;
      private List<string> _stations = null;
      private List<string> _vehicles = null;

      public bool UseTFDDateFilter { get; set; } = false;
      public DateFilterType TFDDateFilterType { get; set; } = DateFilterType.OnDate;

      public string MinDateText { get; set; } = null;
      public string MaxDateText { get; set; } = null;
      public DateTime MinDate { get; set; } = DateTime.MinValue;
      public DateTime MaxDate { get; set; } = DateTime.MaxValue;

      public bool UseTFDProblemFilter { get; set; } = false;
      public List<string> TFDProblems { 
         get { 
            if (_problems == null) { _problems = new List<string>(); } 
            return _problems; 
         }
         set { _problems = value; } 
      }

      public bool UseTFDDivisionFilter { get; set; } = false;
      public List<string> TFDDivsions {
         get {
            if (_divisions == null) { _divisions = new List<string>(); }
            return _divisions;
         }
         set { _divisions = value; }
      }

      public bool UseTFDStationFilter { get; set; } = false;
      public List<string> TFDStations {
         get {
            if (_stations == null) { _stations = new List<string>(); }
            return _stations;
         }
         set { _stations = value; }
      }

      public bool UseTFDVehicleFilter { get; set; } = false;
      public List<string> TFDVehicles {
         get {
            if (_vehicles == null) { _vehicles = new List<string>(); }
            return _vehicles;
         }
         set { _vehicles = value; }
      }
   }
}
