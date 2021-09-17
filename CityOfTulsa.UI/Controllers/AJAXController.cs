using CityOfTulsaUI.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Controllers {

   public class AJAXController : Controller {

      //[Authorize]
      [HttpPost]
      public IActionResult ProcessMessage(
         [FromBody]AJAXMessage ajaxMessage
      ) {

         AJAXPayload ajaxPayload = new AJAXPayload(ajaxMessage);

         try {

            ajaxPayload.timetext = System.DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-fff");

            switch ((ajaxMessage.cmd ?? "").Trim().ToLower()) {

               case "mindate_set":

                  break;

               case "maxdate_set":

                  break;
            }
         }
         catch (System.Exception ex) {

            ajaxPayload.cmd = "error";
            ajaxPayload.origcmd = ajaxMessage.cmd;
            ajaxPayload.value = (-1).ToString();
            ajaxPayload.msg = ex.ToString();
         }

         JsonResult jsonResult = new JsonResult(
            JsonConvert.SerializeObject(ajaxPayload)
            );

         return jsonResult;
      }
   }
}
