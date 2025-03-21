
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.WebMvc.Models;
using SurveyApp.Application.Ports;

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
                _logger.LogInformation($"Attempting login for user: {model.Username}");
                
                // Validate credentials - ensure we use case-insensitive comparison
                bool isValid = await _authService.ValidateUserAsync(model.Username.Trim(), model.Password);
                
                if (isValid)
                {
                    // Get user information
                    var user = await _authService.GetUserByUsernameAsync(model.Username.Trim());
                    
                    if (user == null)
                    {
                        _logger.LogWarning($"User validated but not found: {model.Username}");
                        ModelState.AddModelError(string.Empty, "Error al obtener información del usuario");
                        return View(model);
                    }
                    
                    string userRole = user.Role;
                    string userEmail = user.Email;
                    
                    _logger.LogInformation($"User {model.Username} authenticated successfully with role: {userRole}");
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Email, userEmail),
                        new Claim(ClaimTypes.Role, userRole)
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
                    
                    // Direct user to appropriate page based on role
                    if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                
                _logger.LogWarning($"Invalid login attempt for user {model.Username}");
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error durante el inicio de sesión para usuario {model.Username}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error durante el inicio de sesión. Por favor, inténtelo de nuevo.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Agregamos un mensaje de éxito similar al toast de React
            TempData["SuccessMessage"] = "Has cerrado sesión correctamente";
            
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
                    // Verificar si se intenta registrar con uno de los usuarios predefinidos
                    if (model.Username == "admin" || model.Username == "client")
                    {
                        ModelState.AddModelError(string.Empty, "Este nombre de usuario está reservado");
                        return View(model);
                    }
                    
                    var userExists = await _authService.UserExistsAsync(model.Username);
                    
                    if (userExists)
                    {
                        ModelState.AddModelError(string.Empty, "El nombre de usuario ya está en uso");
                        return View(model);
                    }
                    
                    var success = await _authService.RegisterUserAsync(model.Username, model.Email, model.Password, "Client");
                    
                    if (success)
                    {
                        _logger.LogInformation($"Usuario {model.Username} registrado correctamente");
                        
                        // Agregar mensaje de éxito
                        TempData["SuccessMessage"] = "Su cuenta ha sido creada correctamente";
                        
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
                Role = userRole,
                LastLogin = DateTime.Now.AddDays(-1) // Simulamos última conexión
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
