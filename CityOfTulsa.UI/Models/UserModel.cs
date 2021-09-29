using CityOfTulsaData;
using CityOfTulsaUI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

      private QuerySettings _qrySettings = null;

      public string APIToken { get; set; } = null;

      public bool VerifyAPIToken(string loginUrl) {

         if (!(string.IsNullOrWhiteSpace(this.APIToken))) {
            return true;
         }

         UserAuthInfo userInfo = new UserAuthInfo();
         userInfo.UserName = "generic-client";
         userInfo.Password = "Ciudad de Tulsey 4most o!l-town";

         HttpClient httpClient = new HttpClient();
         var contentType = new MediaTypeWithQualityHeaderValue("application/json");
         httpClient.DefaultRequestHeaders.Accept.Add(contentType);

         string stringData = JsonConvert.SerializeObject(userInfo);
         var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

         HttpResponseMessage response = httpClient.PostAsync(loginUrl, contentData).Result;
         string stringJWT = response.Content.ReadAsStringAsync().Result;
         JWT jwt = JsonConvert.DeserializeObject<JWT>(stringJWT);

         this.APIToken = jwt.Token;

         return true;
      }

      public QuerySettings PrevQuerySettings { get; set; } = null;

      public QuerySettings QuerySettings {
         get {
            if (_qrySettings == null) {
               _qrySettings = new QuerySettings();
            }
            return _qrySettings;
         }
         set {
            _qrySettings = value;
         }
      }

      public void ExecuteQuery() {
         this.PrevQuerySettings = this.QuerySettings.DeepCopy();
      }

      public int TFDEventsCountResult { get; set; } = Int32.MinValue;
   }

   public class QuerySettings {

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

      public QuerySettings ShallowCopy() {
         return (QuerySettings)this.MemberwiseClone();
      }

      public QuerySettings DeepCopy() {
         QuerySettings clone = (QuerySettings)this.MemberwiseClone();
         clone.TFDDivsions = this.TFDDivsions.Clone();
         clone.TFDProblems = this.TFDProblems.Clone();
         clone.TFDStations = this.TFDStations.Clone();
         clone.TFDVehicles = this.TFDVehicles.Clone();
         return clone;
      }

      public DateTime DisplayDate1 {
         get {
            switch (this.TFDDateFilterType) {
               case DateFilterType.AfterDate:
               case DateFilterType.BetweenDates:
               case DateFilterType.OnDate:
                  return this.MinDate;
               case DateFilterType.BeforeDate:
                  return this.MaxDate;
               default:
                  return DateTime.MinValue;
            }
         }
      }

      public DateTime DisplayDate2 {
         get {
            switch (this.TFDDateFilterType) {
               case DateFilterType.BetweenDates:
                  return this.MaxDate;
               default:
                  return DateTime.MinValue;
            }
         }
      }
   }
}
