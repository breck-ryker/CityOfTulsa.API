﻿using CityOfTulsaData;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Classes {

   public static class CommonLib {

      public enum CacheKeys {
         COT_API_TFD_Problems = 1,
         COT_API_TFD_Divisions = 2,
         COT_API_TFD_Stations = 3,
         COT_API_TFD_Vehicles = 4
      }

      public static int validateDateFilters(UserModel model, ref AJAXPayload payload) {

         if (model.UseTFDDateFilter) {

            // about the MinDateText and MaxDateText...and msgmode:
            // it's kind of harsh to complain to the user that their dates are invalid if they've not actually yet entered/selected anything,
            // and they're maybe just clicking around.  Let's not complain until they've entered something OR are trying to submit the search.

            switch (model.TFDDateFilterType) {

               case DateFilterType.OnDate:
               case DateFilterType.AfterDate:
               case DateFilterType.BeforeDate:

                  if (!(model.MinDate.IsValidValue())) {
                     payload.msg = "Date is invalid. Please enter a valid date.";
                     if (string.IsNullOrWhiteSpace(model.MinDateText)) {
                        payload.msgmode = "no-user-entry";
                     }
                     return -1;
                  }

                  break;

               case DateFilterType.BetweenDates:

                  if (!(model.MinDate.IsValidValue())) {
                     payload.msg = "Range start date is invalid. Please enter a valid start date.";
                     if (string.IsNullOrWhiteSpace(model.MinDateText)) {
                        payload.msgmode = "no-user-entry";
                     }
                     return -1;
                  }
                  else if (!(model.MaxDate.IsValidValue())) {
                     payload.msg = "Range end date is invalid. Please enter a valid end date.";
                     if (string.IsNullOrWhiteSpace(model.MaxDateText)) {
                        payload.msgmode = "no-user-entry";
                     }
                     return -1;
                  }
                  else if (model.MaxDate <= model.MinDate) {
                     if (string.IsNullOrWhiteSpace(model.MinDateText) && string.IsNullOrWhiteSpace(model.MaxDateText)) {
                        payload.msgmode = "no-user-entry";
                     }
                     payload.msg = "Range's start date must precede its end date. To use only one date, select the On Date option.";
                     return -1;
                  }

                  break;
            }
         }

         return 0;
      }
   }

   public static class SessionExtensions {

      public static void Set<T>(this ISession session, string key, T value) {
         session.SetString(key, JsonSerializer.Serialize(value));
      }

      public static T Get<T>(this ISession session, string key) {
         var value = session.GetString(key);
         return value == null ? default : JsonSerializer.Deserialize<T>(value);
      }
   }
}
