using EfCoreBasicsMitDbFirstUndAuth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EfCoreBasicsMitDbFirstUndAuth.Data;
using Microsoft.EntityFrameworkCore;
using EfCoreBasicsMitDbFirstUndAuth.Services;

namespace EfCoreBasicsMitDbFirstUndAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EfCoreDbFirstAuthContext _ctx;
        private readonly AuthService _authService;

        public HomeController(ILogger<HomeController> logger, EfCoreDbFirstAuthContext ctx, AuthService authService)
        {
            _logger = logger;
            _ctx = ctx;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            //Kontext NICHT selbst erzeugen, sondern über DI holen
            //var ctx = new EfCoreDbFirstAuthContext();

            ViewBag.NumRoles = await _ctx.AppRoles.CountAsync();

            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            //Wenn die returnUrl null ist, dann leiten wir auf die Index-Seite weiter
            returnUrl ??= "/Home/Index";

            //Benutzername/Email und Passwort überprüft
            var user = await _authService.GetUserIfCanLogInAsync(username, password);

            if(user is not null)
            {
                var claims = new List<Claim>()
                {
                    //Claims sind Key-Value-Paare für Benutzerinformationen
                    //Wichtig: Für Benutzername, Email und Rolle, sollten die eingebauten Bezeichnungen verwendet werden
                    new Claim(ClaimTypes.Name, user.Username),
                    //new Claim(ClaimTypes.Email, "max@muster.at"),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),

                    //aber es können natürlich auch eigene Namen verwendet werden
                    new Claim("LastLogin", DateTime.Now.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProps = new AuthenticationProperties()
                {
                    IsPersistent = true //Belässt das Cookie über mehrere Browser-Sitzungen hinweg
                };

                //Erzeugt das Authentication Cookie und gibt es dem Benutzer mit der Response zurück
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProps);

                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string passwordRepeat)
        {
            //Sind die Passwörter die selben?
            await _authService.RegisterNewUser(username, password);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
