using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UdemyIdentityServer.Client1.Services;

namespace UdemyIdentityServer.Client1
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
            services.AddHttpContextAccessor();
            services.AddScoped<IApiResourceHttpClient, ApiResourceHttpClient>();

            services.AddAuthentication(opts =>
            {

                //Schemalar ne ucun lazimdir. Bizim programimizada birde cox Authentication(uyelik sistemi) ola biler.Bu Authenticatio sistemini
                //bir birinnen ayira bilmey ucun Schema konsepti istifade olunur
                opts.DefaultScheme = "Cookies";

                //bu cookie kimnnen xeberlesecey onu bildirmeliyik asagida kod vasitesi ile
                opts.DefaultChallengeScheme = "oidc";

                //AddCookie("Cookies"),AddOpenIdConnect("oidc") Bu kodlar vasitesi ile mende olan Cookie ile OpenId Connetden gelen 
                //yani Authprozation Serverden  gelen Cookie bir biri ile xeberlesmeye baslayir

            }).AddCookie("Cookies", opts =>
            {
                opts.AccessDeniedPath = "/Home/AccessDenied";
            }).AddOpenIdConnect("oidc", opts =>
            {
                opts.SignInScheme = "Cookies";

                //Cookie ve ya Token paylayan yer. mene bu hardan gonderilecek onu bildiririk
                opts.Authority = "https://localhost:5001";

                opts.ClientId = "Client1-Mvc";
                opts.ClientSecret = "secret";
                opts.ResponseType = "code id_token";

                //Biz bunu set etdiyimiz de gedecey ozu arxa planda UsernInfo Endpointine sorgu atacag ve ordan gelen Claimleri User.Claims-in
                //icine add eliyecey. Bunu true etmediyimiz zaman Userin elave melumatlarina User.Claims uzerinnen catmag olar ve Claim-lere
                //Userin elave melumatlari add olmur.
                opts.GetClaimsFromUserInfoEndpoint = true;

                //Ugurlu bir Authorization prosesinnen sora Authentication Properti-ye Access token ve Refresh tokeni save edecey
                opts.SaveTokens = true;

                opts.Scope.Add("api1.read");

                //Refresh token ucun
                opts.Scope.Add("offline_access");

                opts.Scope.Add("CountryAndCity");
                opts.Scope.Add("Roles");
                opts.ClaimActions.MapUniqueJsonKey("country", "country");
                opts.ClaimActions.MapUniqueJsonKey("city", "city");
                opts.ClaimActions.MapUniqueJsonKey("role", "role");

                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "role"
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
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