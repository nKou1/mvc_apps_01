using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_apps_01.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mvc_apps_01.Jobs;
using Microsoft.AspNetCore.Rewrite;

namespace mvc_apps_01
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
            // 外部認証
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                // Microsoft認証
                microsoftOptions.ClientId = Configuration["AuthenticationMicrosoftClientId"];
                microsoftOptions.ClientSecret = Configuration["AuthenticationMicrosoftClientSecret"];
            }).AddGoogle(googleOptions => {
                // Google認証
                googleOptions.ClientId = Configuration["AuthenticationGoogleClientId"];
                googleOptions.ClientSecret = Configuration["AuthenticationGoogleClientSecret"];
            }).AddFacebook(facebookOptions =>
            {
                // Facebook認証
                facebookOptions.AppId = Configuration["AuthenticationFacebookAppId"];
                facebookOptions.AppSecret = Configuration["AuthenticationFacebookAppSecret"];
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // バックグランド処理
            services.AddHostedService<TimedHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();

                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=TmDbTrendings}/{action=IndexNonAuthorize}/{id?}");
            });
        }
    }
}
