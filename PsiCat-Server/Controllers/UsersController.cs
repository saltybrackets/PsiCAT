namespace PsiCat.Server.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using global::PsiCat.Server.Models;
    

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserDataContext userDataContext;

/*
        public UsersController(UserDataContext userDataContext)
        {
            this.userDataContext = userDataContext;
        }
*/

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }


        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (!this.ModelState.IsValid)
                return BadRequest();
            
            this.userDataContext.Add(user);
            await this.userDataContext.SaveChangesAsync();

            return Json(user);
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