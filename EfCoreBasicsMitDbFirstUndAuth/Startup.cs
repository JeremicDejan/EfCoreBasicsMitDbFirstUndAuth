using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreBasicsMitDbFirstUndAuth.Data;
using EfCoreBasicsMitDbFirstUndAuth.Services;

namespace EfCoreBasicsMitDbFirstUndAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //Dependency Injection findet hier statt, hier werden Dienste im IoC Container registriert
        //Diese können später mittels Constructor-injection angefragt werden
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<EfCoreDbFirstAuthContext>();

            services.AddScoped<AuthService>();

            //Legt fest, dass die Authentifizierungsmethode Cookie-basiert ist
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    //Legt fest, wohin der Benutzer geleitet wird, wenn er nicht eingeloggt ist
                    opts.LoginPath = "/Home/Login";

                    //Legt fest, wie lange das Cookie gültig ist
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                    //Automatischer Refresh der Zeitspanne bei jeder Benutzerinteraktion (die einen Request schickt)
                    opts.SlidingExpiration = true;

                    //Legt fest, wohin der Benutzer geleitet wird, wenn seine Rechte nicht ausreichen
                    //(zB wenn er in der falschen Rolle ist)
                    opts.AccessDeniedPath = "/Home/Error";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Hier wird die Reihenfolge der Middlewares in der HTTP Request Pipeline konfiguriert
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //Fügt die Authentication-Middleware hinzu - sollte VOR der Authorisierung stattfinden
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
