using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SehirRehberi.API.Data;
using SehirRehberi.API.Models;

namespace SehirRehberi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        DataContext _DbContext;
        public ValuesController(DataContext dbContext)
        {
            _DbContext = dbContext;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
       {
            //using (var context = new DataContext())
            //{
            //    return context.Value.ToList();
            //}
            var result = await _DbContext.Value.ToListAsync();
            return Ok(result);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public  ActionResult<Value> Get(int id)
        {
            using (var context = new DataContext())
            {
                return context.Value.Where(v => v.ID == id).SingleOrDefault();
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
