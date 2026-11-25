using BiteFlow.Web.Data;
using BiteFlow.Web.Services.Implementations;
using BiteFlow.Web.Services.Interfaces;

namespace BiteFlow.Web.Services;

/// <summary>
/// Registro de dependencias (inyección). Un método por módulo: cada integrante registra aquí
/// sus propios servicios nuevos dentro de SU método, para evitar conflictos de Git.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddBiteFlowServices(this IServiceCollection services)
    {
        return services
            .AddSeguridadModule()
            .AddCatalogoModule()
            .AddPedidosModule()
            .AddCajaModule();
    }

    // Módulo 1 - Usuarios y seguridad (Javier)
    public static IServiceCollection AddSeguridadModule(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClienteService, ClienteService>();
        return services;
    }

    // Módulo 2 - Productos y categorías (Valeria)
    public static IServiceCollection AddCatalogoModule(this IServiceCollection services)
    {
        services.AddScoped<ICatalogoService, CatalogoService>();
        return services;
    }

    // Módulo 3 - Carrito y pedidos (Raúl)
    public static IServiceCollection AddPedidosModule(this IServiceCollection services)
    {
        services.AddScoped<ICarritoService, CarritoService>();
        services.AddScoped<IPedidoService, PedidoService>();
        return services;
    }

    // Módulo 4 - Caja, pagos y reportes (Montaño)
    public static IServiceCollection AddCajaModule(this IServiceCollection services)
    {
        services.AddScoped<PagoRepository>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddScoped<IReporteVentasService, ReporteVentasService>();
        return services;
    }
}
