using System.Security.Claims;
using BiteFlow.Web.Models;
using BiteFlow.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiteFlow.Web.Controllers;
public class AccountController : Controller
{
    private readonly AuthRepository _auth;
    public AccountController(AuthRepository auth) => _auth = auth;
    [AllowAnonymous] public IActionResult Login(string? returnUrl = null) { ViewBag.ReturnUrl = returnUrl; return View(); }
    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null, CancellationToken ct = default)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid) return View(model);
        var user = await _auth.ValidarAsync(model.Correo, model.Password, ct);
        if (user == null) { ModelState.AddModelError("", "Correo o contraseña incorrectos."); return View(model); }
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
            new(ClaimTypes.Name, $"{user.Nombres} {user.Apellidos}"),
            new(ClaimTypes.Email, user.Correo),
            new(ClaimTypes.Role, user.Rol!.Nombre)
        };
        if (user.Cliente is not null)
            claims.Add(new Claim("IdCliente", user.Cliente.IdCliente.ToString()));
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = model.Recordarme });
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return user.Rol.Nombre switch { Roles.Administrador => RedirectToAction("Admin", "Home"), Roles.Cajero => RedirectToAction("Index", "Pagos"), _ => RedirectToAction("Index", "Home") };
    }
    [HttpGet, AllowAnonymous] public IActionResult Register() => View(new RegistroClienteViewModel());
    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistroClienteViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        try { await _auth.RegistrarClienteAsync(model.Nombres, model.Apellidos, model.Correo, model.Password, model.Telefono, model.Direccion, ct); TempData["Success"] = "Cuenta creada. Ya puedes iniciar sesión."; return RedirectToAction(nameof(Login)); }
        catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); return View(model); }
    }
    [Authorize, HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); return RedirectToAction("Index", "Home"); }
    [AllowAnonymous] public IActionResult AccessDenied() => View();
}
