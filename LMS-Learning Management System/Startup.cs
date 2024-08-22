using LMS_Learning_Management_System.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Identity.CustomPolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IISIntegration;

namespace LMS_Learning_Management_System
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LMSContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

           
            services.AddDetection();


            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(365);
                options.Lockout.MaxFailedAccessAttempts = 9;
                options.Lockout.AllowedForNewUsers = true;

            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

           services.Configure<IdentityOptions>(opts =>
            {
                opts.Lockout.AllowedForNewUsers = true;
                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365);
                opts.Lockout.MaxFailedAccessAttempts = 9;
            });

            services.Configure<DataProtectionTokenProviderOptions>(opts =>
    opts.TokenLifespan = TimeSpan.FromDays(365));


            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/login"); // Specify your login path
                    options.AccessDeniedPath = "/Account/AccessDenied"; // Specify your access denied path
                    options.SlidingExpiration = true; // Enable sliding expiration if needed
                    options.Cookie.SameSite = SameSiteMode.Lax; // Adjust SameSite policy as needed
                    options.Cookie.HttpOnly = true; // Secure cookies by making them HttpOnly
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforce HTTPS for cookies
                    options.Cookie.IsEssential = true; // Mark the cookie as essential for authentication
                    options.Cookie.Name = ".AspNetCore.Identity.Application3"; // Provide a custom name for the authentication cookie
                    options.Cookie.MaxAge = TimeSpan.FromDays(365); // Set cookie expiration time
                    options.Cookie.Expiration = TimeSpan.FromDays(365); // Specify expiration period for the cookie
                });



            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNetCore.Identity.Application3"; // Set your custom cookie name
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None; // Adjust SameSite policy as needed
                options.ExpireTimeSpan = TimeSpan.FromDays(365); // Set the cookie expiration to one year
                options.Cookie.MaxAge = TimeSpan.FromDays(365); // Alternatively, use MaxAge to set relative expiration time
                options.SlidingExpiration = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforce HTTPS for cookies
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".AspNetCore.Identity.Application4"; // Set your custom cookie name
                options.IOTimeout = TimeSpan.FromDays(365); // Set the cookie expiration to one year
                options.IdleTimeout = TimeSpan.FromDays(365); // Session timeout to match cookie expiration
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use HTTPS for cookies
                options.Cookie.SameSite = SameSiteMode.None; // Adjust based on your needs
            });

            //services.AddDistributedMemoryCache();

            services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Arabic }));
            services.AddRazorPages();
            services.AddControllersWithViews();

            services.AddControllersWithViews()
                    .AddNewtonsoftJson(options =>
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AspManager", policy =>
            //    {
            //        policy.RequireRole("Manager");
            //        policy.RequireClaim("Coding-Skill", "ASP.NET Core MVC");
            //    });

            //    options.AddPolicy("AllowTom", policy =>
            //    {
            //        policy.AddRequirements(new AllowUserPolicy("rami_roosan07@yahoo.com"));
            //    });

            //    options.AddPolicy("PrivateAccess", policy =>
            //    {
            //        policy.AddRequirements(new AllowPrivatePolicy());
            //    });
            //});

            //services.AddTransient<IAuthorizationHandler, AllowUsersHandler>();
            //services.AddTransient<IAuthorizationHandler, AllowPrivateHandler>();


            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

            services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Add session middleware
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseCookiePolicy();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "ForGetAllLessons",
                    pattern: "{controller=Lessons}/{action=ShowLessons}/{ClassId?}/{SubjectId?}/{TeacherId?}");
            });
        }
    }
}






//using Identity.CustomPolicy;
//using LMS_Learning_Management_System.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.AspNetCore.HttpsPolicy;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Owin.Security.Cookies;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text.Encodings.Web;
//using System.Text.Unicode;
//using System.Threading.Tasks;

//namespace LMS_Learning_Management_System
//{
//    public class Startup
//    {
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public IConfiguration Configuration { get; }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddControllersWithViews();

//            services.Configure<ForwardedHeadersOptions>(o => {
//                o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
//            });
//            services.AddAntiforgery(options =>
//            {
//                options.HeaderName = "X-XSRF-TOKEN";
//            });
//            // Configure cookie authentication

//            services.ConfigureApplicationCookie(options =>
//            {
//                options.Cookie.HttpOnly = true;
//                options.ExpireTimeSpan = TimeSpan.FromDays(20); // Set cookie expiration to 14 days
//                options.SlidingExpiration = true; // Renew cookie on each request
//                options.LoginPath = "/Academy/Account/Login";
//                options.LogoutPath = "/Academy/Account/Logout";
//                options.AccessDeniedPath = "/Academy/Account/AccessDenied";
//            });

//            services.AddSession(options =>
//            {
//                options.IdleTimeout = TimeSpan.FromDays(20); // Set the idle timeout to 20 days
//                options.Cookie.HttpOnly = true;
//                options.Cookie.IsEssential = true;
//            });
//            services.AddDistributedMemoryCache();

//            //services.ConfigureApplicationCookie(options =>
//            //{
//            //    options.Cookie.HttpOnly = true;
//            //    options.ExpireTimeSpan = TimeSpan.FromDays(14);
//            //    options.LoginPath = "/Academy/Account/Login";
//            //    options.AccessDeniedPath = "/Academy/Account/AccessDenied";
//            //    options.SlidingExpiration = true;
//            //    // Add event handler for OnRedirectToLogin
//            //    options.Events = new CookieAuthenticationEvents
//            //    {
//            //        OnRedirectToLogout = context =>
//            //        {
//            //            // Check if the user is being redirected due to an expired cookie
//            //            if (context.Request.Path.StartsWithSegments("/Academy/Account/Login")
//            //                && context.Response.StatusCode == (int)HttpStatusCode.OK)
//            //            {
//            //                // Redirect to a specific page after ExpireTimeSpan has elapsed
//            //                context.Response.Redirect("/Academy/Account/logout");
//            //            }
//            //            else
//            //            {
//            //                // Default behavior (redirect to the login page)
//            //                context.Response.Redirect("/Academy/Account/logout");
//            //                //context.Response.Redirect(context.RedirectUri);
//            //            }

//            //            return Task.CompletedTask;
//            //        }
//            //    };
//            //});

//            //services.AddAuthentication(options =>
//            //{
//            //    options.DefaultAuthenticateScheme = "Cookies";
//            //    options.DefaultSignInScheme = "Cookies";
//            //    options.DefaultSignOutScheme = "Cookies";
//            //})
//            //    .AddCookie(options =>
//            //    {
//            //        options.Cookie.Name = ".AspNetCore.Identity.Application22";
//            //        options.ExpireTimeSpan = TimeSpan.FromDays(99999); // Set your desired session timeout
//            //        options.SlidingExpiration = true;

//            //        options.Events.OnSigningOut = context =>
//            //        {
//            //            context.Response.StatusCode = 401; // Unauthorized status code
//            //            return Task.CompletedTask;
//            //        };
//            //    });

//            //services.AddSession(options =>
//            //{
//            //    //options.IdleTimeout = TimeSpan.Zero; // Set your desired session timeout
//            //    options.IdleTimeout = TimeSpan.FromMinutes(3330); ; // Set your desired session timeout
//            //    //options.Cookie.HttpOnly = true;

//            //});


//            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Arabic }));


//            services.AddControllersWithViews()
//                     .AddNewtonsoftJson(options =>
//                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//             );


//            services.AddDistributedMemoryCache();


//            services.AddDbContext<LMSContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
//            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
//            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

//            services.AddDetection();
//            services.AddAuthorization(opts =>
//            {
//                opts.AddPolicy("AspManager", policy =>
//                {
//                    policy.RequireRole("Manager");
//                    policy.RequireClaim("Coding-Skill", "ASP.NET Core MVC");
//                });
//            });

//            services.AddTransient<IAuthorizationHandler, AllowUsersHandler>();
//            services.AddAuthorization(opts =>
//            {
//                opts.AddPolicy("AllowTom", policy =>
//                {
//                    policy.AddRequirements(new AllowUserPolicy("rami_roosan07@yahoo.com"));
//                });
//            });

//            services.AddTransient<IAuthorizationHandler, AllowPrivateHandler>();
//            services.AddAuthorization(opts =>
//            {
//                opts.AddPolicy("PrivateAccess", policy =>
//                {
//                    policy.AddRequirements(new AllowPrivatePolicy());
//                });
//            });

//            services.Configure<MvcNewtonsoftJsonOptions>(options =>
//            {
//                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//            });




//            services.AddRazorPages();




//            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None };

//            services.AddControllers().AddJsonOptions(jsonOptions =>
//            {
//                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
//            });

//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            app.UseCookiePolicy();
//            app.UseAuthentication();
//            app.UseSession(); // Add session middleware


//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseExceptionHandler("/Home/Error");
//                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                app.UseHsts();
//            }

//            app.UseForwardedHeaders();
//            app.UseHttpsRedirection();
//            app.UseStaticFiles();

//            app.UseRouting();

//            app.UseAuthorization();

//            //app.UseMiddleware<KeepAliveMiddleware>();
//            //app.UseMiddleware<SecurityStampMiddleware>();


//            //app.Run(async (context) => {
//            //    var msg = Configuration["message"];
//            //    await context.Response.WriteAsync(msg);
//            //});
//            app.UseEndpoints(endpoints =>
//            {
//                endpoints
//       .MapControllers()
//       .RequireAuthorization();
//                endpoints.MapRazorPages();
//                endpoints.MapControllerRoute(
//                    name: "default",
//                    pattern: "{controller=Home}/{action=Index}/{id?}");

//                //endpoints.MapControllerRoute(
//                //   name: "SpicificRoute",
//                //   pattern: "{var}/{var}/{controller=Lessons}/{var}/{action=_PartialShowLessons_D}/{var}/{var}/{var}");

//                endpoints.MapControllerRoute(
//                    name: "ForGetAllLessons",
//                    pattern: "{controller=Lessons}/{action=ShowLessons}/{ClassId?}/{SubjectId?}/{TeacherId?}");

//            });


//        }
//    }
//}
