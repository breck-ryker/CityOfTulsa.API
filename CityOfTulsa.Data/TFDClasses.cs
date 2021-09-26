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

      public FireEventHelper(FireEvent fe) {

         if (fe == null) { return; }

         this.FireEventID = fe.FireEventID;
         this.IncidentNumber = fe.IncidentNumber;
         this.Problem = fe.Problem;
         this.Address = fe.Address;
         this.ResponseDate = fe.ResponseDate;
         this.Latitude = fe.Latitude;
         this.Longitude = fe.Longitude;
         this.DateLastUpdated = fe.DateLastUpdated;

         if (fe.FireVehicles != null && fe.FireVehicles.Count > 0) {

            this.FireVehicles = new List<FireVehicleHelper>();

            foreach (FireVehicle fv in fe.FireVehicles) {
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

      public FireVehicleHelper(FireVehicle fv) {

         if (fv == null) { return; }

         this.FireVehicleID = fv.FireVehicleID;
         this.Division = fv.Division;
         this.Station = fv.Station;
         this.VehicleID = fv.VehicleID;
         this.DateLastUpdated = fv.DateLastUpdated;
      }

      public int FireVehicleID { get; set; }
      public string Division { get; set; }
      public string Station { get; set; }
      public string VehicleID { get; set; }
      public DateTime DateLastUpdated { get; set; }
   }
}
