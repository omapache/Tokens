using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Services;
using Aplicacion.UnitOfWork;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
         public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
        public static void AddAplicacionServices(this IServiceCollection services)
            {
                //Services.AddScoped<IpaisInterface,PaisRepository>();
                //Services.AddScoped<ITipoPersona,TipoPeronsaRepository>();

                services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>(); 
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<IAuthorizationHandler, GlobalVerbRoleHandlre>();
            }
    }
}