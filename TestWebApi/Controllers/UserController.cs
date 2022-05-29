using Microsoft.AspNetCore.Mvc;
using TestWebApi.Context;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("controller")]
    public class UserController : ControllerBase
    {
        readonly DbFileContext db;
        public UserController(DbFileContext _db)
        {
            db = _db;   
        }

      //  [HttpGet]

    }
}
