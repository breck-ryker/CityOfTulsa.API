using CityOfTulsaUI.Classes;
using CityOfTulsaUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Controllers {

   public class AJAXController : Controller {

      [HttpPost]
      public IActionResult ProcessMessage(
         [FromBody]AJAXMessage ajaxMessage
      ) {

         AJAXPayload ajaxPayload = new AJAXPayload(ajaxMessage);
         UserModel userModel = HttpContext.Session.Get<UserModel>("UserModel");
         
         if (userModel == null) {
            userModel = new UserModel();
         }

         try {

            ajaxPayload.timetext = System.DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-fff");

            switch ((ajaxMessage.cmd ?? "").Trim().ToLower()) {

               case "tfd.set-mindate":

                  break;

               case "tfd.set-maxdate":

                  break;

               case "tfd.show-dateoptions":

                  userModel.UseTFDDateFilter = (ajaxMessage.data.ToInteger() <= 0 ? false : true);

                  break;

               case "tfd.set-dateoption":

                  userModel.TFDDateFilterType = (DateFilterType)ajaxMessage.data.ToInteger();

                  break;

               case "tfd.show-problemlist":

                  userModel.UseTFDProblemFilter = (ajaxMessage.data.ToInteger() <= 0 ? false : true);

                  break;
            }
         }
         catch (System.Exception ex) {

            ajaxPayload.cmd = "error";
            ajaxPayload.origcmd = ajaxMessage.cmd;
            ajaxPayload.value = (-1).ToString();
            ajaxPayload.msg = ex.ToString();
         }

         HttpContext.Session.Set("UserModel", userModel);

         JsonResult jsonResult = new JsonResult(
            JsonConvert.SerializeObject(ajaxPayload)
            );

         return jsonResult;
      }
   }
}
