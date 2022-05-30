using Microsoft.AspNetCore.Mvc;
using TestWebApi.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestWebApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TestWebApi.Controllers
{
   
    [Route("[controller]")]
    public class UsersController : Controller
    {
        readonly DbFileContext db;
        public UsersController(DbFileContext _db)
        {
            db = _db;   
        }

        [HttpGet]
        public string Login()
        {
            return "Нужна авторизация!";

        }

        [HttpPost]
        
        public async Task<ActionResult> Registration(AuthorizeModel model)
        {
            
            //if (ModelState.IsValid)
            //{
                User newuser =await db.Users.FirstOrDefaultAsync(u=>u.Login==model.Login);
                
                if (newuser==null)
                {
                    newuser = new User { Login = model.Login, Password = model.Password };
                    db.Users.Add(newuser);
                    await db.SaveChangesAsync();
                    await Authenticate(model);
                    return Ok(model);
                }
                //else
                //{
                //    ModelState.AddModelError("", "Некорректные логин или пароль");
                    
                //}
            throw new Exception($"{newuser.Login} {newuser.Password}");
            //}
            return  BadRequest(model);
        }


        private async Task Authenticate(AuthorizeModel model)
        {
            var claims = new List<Claim>();
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, model.Login);
            }

            ClaimsIdentity id = new ClaimsIdentity(claims,"ApplicationCookie",ClaimsIdentity.DefaultNameClaimType,ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logout");
        }

    }
}
