using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogary.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blogary
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
            services.AddControllersWithViews();
         
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer("Data Source=tcp:blogarydbserver.database.windows.net,1433;Initial Catalog=Blogary_db;User Id=BlogaryAdmin@blogarydbserver;Password=Spoonzio!")                                    // CONNECTION STRING GOES HERE
            );

            // Identity user and roles
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.SignIn.RequireConfirmedEmail = true;

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // MVC
            services.AddMvc(option => {
                option.EnableEndpointRouting = false;
                //Policies
            });

            services.AddScoped<IBlogRepository, SQLBlogRepository>();
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
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            // Serve static files
            app.UseStaticFiles();

            // Authentication
            app.UseAuthentication();

            // Route
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}");
            });
        }
    }
}
