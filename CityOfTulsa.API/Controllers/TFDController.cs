using CityOfTulsaData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityOfTulsaAPI.Controllers {

   [Route("[controller]")]
   [ApiController]
   public class TFDController : ControllerBase {

      private readonly DatabaseContext _context;

      public TFDController(DatabaseContext context) {
         _context = context;
      }

      // GET: api/<TFDController>
      [HttpGet]
      public IEnumerable<FireEventHelper> Get() {
         
         return _context.FireEvents
            .Where(e => e.ResponseDate >= DateTime.Now.AddDays(-7))
            .Include(e => e.FireVehicles)
            .Select(e => new FireEventHelper(e))
            ;
      }

      // GET api/<TFDController>/5
      [HttpGet("{id}")]
      public string Get(int id) {
         return "value";
      }

      // POST api/<TFDController>
      [HttpPost]
      public void Post([FromBody] string value) {
      }

      // PUT api/<TFDController>/5
      [HttpPut("{id}")]
      public void Put(int id, [FromBody] string value) {
      }

      // DELETE api/<TFDController>/5
      [HttpDelete("{id}")]
      public void Delete(int id) {
      }
   }
}
