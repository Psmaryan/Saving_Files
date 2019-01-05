using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SavingFile.Models;
using SavingFile.ViewModels;
using SavingFile.Util;

namespace SavingFile.Controllers
{
 
    
    public class AccountController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

      
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        private List<User> people = new List<User>
        {
           // new User {Login="admin@gmail.com", Password="12345" },
           // new User { Login="qwerty", Password="55555" }
        };

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Login = model.Email, UserName = model.Email, Email = model.Email };

                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    //await _signInManager.SignInAsync(user, false);
                    return Redirect("~/Account/Index"); 
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return PartialView();
        }

        [HttpPost("/token")]
        public async Task Token()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];
            await _signInManager.PasswordSignInAsync(username, password, false, false);

            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            User usr = await _userManager.FindByNameAsync(username);
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                id = usr.Id
            };

            // сериализация ответа
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            User usr = await _userManager.FindByNameAsync(username);
            if (usr != null)
            {
                bool result = await _userManager.CheckPasswordAsync(usr, password);

                if (result)
                {
                    User person = new User { Login = username };
                    if (person != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login)

                        };

                        ClaimsIdentity claimsIdentity =
                        new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType);
                        return claimsIdentity;
                    }
                }
            }
            // если пользователя не найдено
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return View("Index");
        }

    }
}