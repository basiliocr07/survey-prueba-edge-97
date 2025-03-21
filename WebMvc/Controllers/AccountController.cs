
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Ports;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthenticationService authService,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var isValid = await _authService.ValidateUserCredentialsAsync(model.Username, model.Password);
                
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
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation($"Usuario {model.Username} ha iniciado sesión correctamente");
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    
                    return RedirectToAction("Index", "Dashboard");
                }
                
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el inicio de sesión");
                ModelState.AddModelError(string.Empty, "Ocurrió un error durante el inicio de sesión. Por favor, inténtelo de nuevo.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userExists = await _authService.UserExistsAsync(model.Username);
                    
                    if (userExists)
                    {
                        ModelState.AddModelError(string.Empty, "El nombre de usuario ya está en uso");
                        return View(model);
                    }
                    
                    // Asignar el rol predeterminado (Cliente)
                    var success = await _authService.CreateUserAsync(model.Username, model.Email, model.Password, "Client");
                    
                    if (success)
                    {
                        _logger.LogInformation($"Usuario {model.Username} registrado correctamente");
                        return RedirectToAction("Login");
                    }
                    
                    ModelState.AddModelError(string.Empty, "Error al crear el usuario");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el registro de usuario");
                    ModelState.AddModelError(string.Empty, "Ocurrió un error durante el registro. Por favor, inténtelo de nuevo.");
                }
            }
            
            return View(model);
        }

        [Authorize]
        public IActionResult Profile()
        {
            var username = User.Identity.Name;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            var model = new UserProfileViewModel
            {
                Username = username,
                Email = userEmail,
                Role = userRole
            };
            
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
