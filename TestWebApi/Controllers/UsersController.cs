using Microsoft.AspNetCore.Mvc;
using TestWebApi.Context;
using Microsoft.EntityFrameworkCore;
using TestWebApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        readonly DbFileContext db;
        public UsersController(DbFileContext _db)
        {
            db = _db;   
        }

        [HttpGet]
        public string redirect()
        {
            return "Нужна авторизация!";

        }

        [HttpPost]
        [Route("registration")]
        public async Task<ActionResult> Registration(AuthorizeModel model)
        {

            if (ModelState.IsValid)
            {
                User newuser = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

                if (newuser == null)
                {
                    newuser = new User { Login = model.Login, Password = model.Password };
                    db.Users.Add(newuser);
                    await db.SaveChangesAsync();
                    await Authenticate(model);
                    return RedirectToAction("getall", "dbfiles");

                }
                else
                {
                    return BadRequest("Такой пользователь уже существует!");

                }

            }
            return BadRequest("Все поля должны быть заполнены!");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(AuthorizeModel model)
        {
            if (ModelState.IsValid)
            {
                User loguser = await db.Users.FirstOrDefaultAsync(u=>u.Login==model.Login&&u.Password==model.Password);
                if (loguser!=null)
                {
                    await Authenticate(model);
                    return RedirectToAction("getall", "dbfiles");
                }
                else
                {
                    return BadRequest("Проверьте правильность ввода логина и пароля");
                }
            }

            return BadRequest("Все поля должны быть заполнены");
        }


        private async Task Authenticate(AuthorizeModel model)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, model.Login)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [Route("logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logout");
        }

    }
}
