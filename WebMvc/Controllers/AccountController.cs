
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.WebMvc.Models;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Services;

namespace SurveyApp.WebMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthenticationService _authService;

        public AccountController(ILogger<AccountController> logger, IAuthenticationService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isValid = await _authService.ValidateUserAsync(model.Username, model.Password);
                
                if (isValid)
                {
                    var user = await _authService.GetUserByUsernameAsync(model.Username);
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation($"Usuario {model.Username} ha iniciado sesión");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe
                if (await _authService.UserExistsAsync(model.Username))
                {
                    ModelState.AddModelError(string.Empty, "Este nombre de usuario ya está en uso.");
                    return View(model);
                }

                var result = await _authService.RegisterUserAsync(
                    model.Username,
                    model.Email,
                    model.Password,
                    model.Role);

                if (result)
                {
                    // Obtener el usuario registrado
                    var user = await _authService.GetUserByUsernameAsync(model.Username);

                    // Iniciar sesión automáticamente
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    _logger.LogInformation($"Nuevo usuario {model.Username} registrado e inició sesión");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al registrar el usuario. Inténtelo de nuevo.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Usuario ha cerrado sesión");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
