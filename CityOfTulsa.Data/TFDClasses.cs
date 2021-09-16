using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CityOfTulsaData {

   public class FireEvent {

      [Required]
      public int FireEventID { get; set; }
      public string IncidentNumber { get; set; }
      public string Problem { get; set; }
      public string Address { get; set; }
      public DateTime ResponseDate { get; set; }
      public decimal Latitude { get; set; }
      public decimal Longitude { get; set; }
      public DateTime DateLastUpdated { get; set; }

      public List<FireVehicle> FireVehicles { get; set; }  // private set
   }

   public class FireVehicle {

      [Required]
      public int FireVehicleID { get; set; }
      public string Division { get; set; }
      public string Station { get; set; }
      public string VehicleID { get; set; }
      public DateTime DateLastUpdated { get; set; }
      
      public List<FireEvent> FireEvents { get; set; }  // private set
   }

   public class FireEventVehicle {

      [Required]
      public int FireEventID { get; set; }
      [Required]
      public int FireVehicleID { get; set; }
      public DateTime DateLastUpdated { get; set; }

      //relationships

      public FireEvent FireEvent { get; set; }  // private set
      public FireVehicle FireVehicle { get; set; }  // private set
   }

   public class FireEventHelper {

      public FireEventHelper(FireEvent fireEvent) {
         this.FireEventID = fireEvent.FireEventID;
         this.IncidentNumber = fireEvent.IncidentNumber;
         this.Problem = fireEvent.Problem;
         this.Address = fireEvent.Address;
         this.ResponseDate = fireEvent.ResponseDate;
         this.Latitude = fireEvent.Latitude;
         this.Longitude = fireEvent.Longitude;
         this.DateLastUpdated = fireEvent.DateLastUpdated;

         if (fireEvent.FireVehicles != null && fireEvent.FireVehicles.Count > 0) {

            this.FireVehicles = new List<FireVehicleHelper>();

            foreach (FireVehicle fv in fireEvent.FireVehicles) {
               this.FireVehicles.Add(new FireVehicleHelper(fv));
            }
         }
      }

      public int FireEventID { get; set; }
      public string IncidentNumber { get; set; }
      public string Problem { get; set; }
      public string Address { get; set; }
      public DateTime ResponseDate { get; set; }
      public decimal Latitude { get; set; }
      public decimal Longitude { get; set; }
      public DateTime DateLastUpdated { get; set; }

      public List<FireVehicleHelper> FireVehicles { get; set; }  // private set
   }

   public class FireVehicleHelper {

      public FireVehicleHelper(FireVehicle fireVehicle) {
         this.FireVehicleID = fireVehicle.FireVehicleID;
         this.Division = fireVehicle.Division;
         this.Station = fireVehicle.Station;
         this.VehicleID = fireVehicle.VehicleID;
         this.DateLastUpdated = fireVehicle.DateLastUpdated;
      }

      public int FireVehicleID { get; set; }
      public string Division { get; set; }
      public string Station { get; set; }
      public string VehicleID { get; set; }
      public DateTime DateLastUpdated { get; set; }
   }
}
