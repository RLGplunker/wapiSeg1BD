using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using wapiSeg1BD.Models;
using wapiSeg1BD.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using wapiSeg1BD.Interfaces;
using wapiSeg1BD.Services;
using wapiSeg1BD.ModelDb;
using AutoMapper;
using Newtonsoft.Json;

namespace wapiSeg1BD
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Usar serialización NewtonSoft
            services.AddControllers().AddNewtonsoftJson(opt => {
                opt.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                opt.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });


            //Servicio de encriptamiento 
            services.AddDataProtection();

            // CORS POR MIDDLEWARE
            // services.AddCors();

            // CORS POR POLÍTICAS DE SEGURIDAD
            services.AddCors(options => 
                options.AddPolicy("Permitir_Aplic_Mvc_Rosana"
                , builder => builder
                     .WithOrigins("https://localhost:5000")
                     .WithMethods("GET", "POST")
                     .AllowAnyHeader()));
                    
            // BD-IDENTITY
            services.AddDbContext<ApplicationDbContext>(opt => 
                opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            //AUTHENTICATION
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                         ClockSkew = TimeSpan.Zero
                     });

            services.AddControllers();

            //DI CUSTOM (no propias del framework)
            services.AddSingleton<IHashService,HashService>();

            //AUTOMAPPER
            services.AddAutoMapper(typeof(wapiSeg1BD.Mappers.AutoMapping));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();         

            app.UseAuthentication();

            app.UseAuthorization();

            // CORS POR MIDDLEWARE
            //app.UseCors(builder => builder
            // .WithOrigins("https://www.apirequest.io")
            // .WithMethods("GET","POST")
            // .AllowAnyHeader());

            // CORS POR POLÍTICAS DE SEGURIDAD
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
