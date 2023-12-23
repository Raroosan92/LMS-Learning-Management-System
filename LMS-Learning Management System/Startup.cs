using Identity.CustomPolicy;
using LMS_Learning_Management_System.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace LMS_Learning_Management_System
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
            // Configure cookie authentication
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(300);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                // Add event handler for OnRedirectToLogin
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogout = context =>
                    {
                        // Check if the user is being redirected due to an expired cookie
                        if (context.Request.Path.StartsWithSegments("/Account/Login")
                            && context.Response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            // Redirect to a specific page after ExpireTimeSpan has elapsed
                            context.Response.Redirect("/Account/logout");
                        }
                        else
                        {
                            // Default behavior (redirect to the login page)
                            context.Response.Redirect("/Account/logout");
                            //context.Response.Redirect(context.RedirectUri);
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = "Cookies";
            //    options.DefaultSignInScheme = "Cookies";
            //    options.DefaultSignOutScheme = "Cookies";
            //})
            //    .AddCookie(options =>
            //    {
            //        options.Cookie.Name = ".AspNetCore.Identity.Application22";
            //        options.ExpireTimeSpan = TimeSpan.Zero; // Set your desired session timeout
            //options.SlidingExpiration = true;

            //        options.Events.OnSigningOut = context =>
            //        {
            //            context.Response.StatusCode = 401; // Unauthorized status code
            //    return Task.CompletedTask;
            //        };
            //    });

            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.Zero; // Set your desired session timeout
            //    options.Cookie.HttpOnly = true;

            //});


            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Arabic }));


            services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

            //services.AddDistributedMemoryCache();
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromSeconds(10);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});


            //services.AddDbContext<ModelContext>(options =>
            //   options.UseOracle(
            //         Configuration.GetConnectionString("DefaultConnection"), b => b.UseOracleSQLCompatibility("11")));

            //     services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationType)
            //.AddCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //    options.AccessDeniedPath = "/Account/AccessDenied";
            //    // Additional options...
            //});
            services.AddDbContext<LMSContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));


            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();


            //services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));
            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.Cookie.Name = ".AspNetCore.Identity.Application";
            //    options.ExpireTimeSpan = TimeSpan.FromDays(3900);
            //    options.SlidingExpiration = true;
            //});

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("AspManager", policy =>
                {
                    policy.RequireRole("Manager");
                    policy.RequireClaim("Coding-Skill", "ASP.NET Core MVC");
                });
            });

            services.AddTransient<IAuthorizationHandler, AllowUsersHandler>();
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("AllowTom", policy =>
                {
                    policy.AddRequirements(new AllowUserPolicy("rami_roosan07@yahoo.com"));
                });
            });

            services.AddTransient<IAuthorizationHandler, AllowPrivateHandler>();
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("PrivateAccess", policy =>
                {
                    policy.AddRequirements(new AllowPrivatePolicy());
                });
            });

            //services.AddAuthentication().AddGoogle(opts =>
            //{
            //    opts.ClientId = "717469225962-3vk00r8tglnbts1cgc4j1afqb358o8nj.apps.googleusercontent.com";
            //    opts.ClientSecret = "babQzWPLGwfOQVi0EYR-7Fbb";
            //    opts.SignInScheme = IdentityConstants.ExternalScheme;
            //});

            //services.Configure<IdentityOptions>(opts =>
            //{
            //    opts.Lockout.AllowedForNewUsers = false;
            //    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            //    opts.Lockout.MaxFailedAccessAttempts = 3;

            //});

            /*services.Configure<IdentityOptions>(opts =>
            {
                opts.SignIn.RequireConfirmedEmail = true;
            });*/




            services.AddRazorPages();



            services.AddControllersWithViews();

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None };

            services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            //app.UseSession();

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

            app.UseAuthorization();


            //app.UseMiddleware<SecurityStampMiddleware>();


            //app.Run(async (context) => {
            //    var msg = Configuration["message"];
            //    await context.Response.WriteAsync(msg);
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints
       .MapControllers()
       .RequireAuthorization();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


                //endpoints.MapControllerRoute(
                //    name: "ForReports",
                //    pattern: "{controller=Reports}/{action=Spend_Income_EL_Orders_Markets_RPT}/{no?}/{Checked_Markets?}");

            });


        }
    }
}
