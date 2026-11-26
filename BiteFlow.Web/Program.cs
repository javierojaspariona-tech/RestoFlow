using BiteFlow.Web.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; options.IdleTimeout = TimeSpan.FromHours(2); });
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<ProductoRepository>();
builder.Services.AddScoped<PedidoRepository>();
builder.Services.AddScoped<CarritoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<PagoRepository>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "BiteFlow.Auth"; options.SlidingExpiration = true; options.ExpireTimeSpan = TimeSpan.FromHours(8);
});
builder.Services.AddAuthorization();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseSession(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
