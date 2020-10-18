using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeveloperCase.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeveloperCase
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers().AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddDbContext<SalesContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("DefaultConnection"), sqloptions => sqloptions.CommandTimeout(3000)).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddRazorPages();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzM2MzM4QDMxMzgyZTMzMmUzMFVhL3JrTXBVOTZzS1c2M1hycnlPaWlRSUFtN3dYcmdzVWFaKzdZNUxiZlk9");
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        //Scaffold-DbContext "Server=.;Database=DeveloperCase;User Id=sa;password=root85;Trusted_Connection=False;MultipleActiveResultSets=true;" Microsoft.EntityFrameworkCore.SqlServer -o Models -f -context "SalesContext"
    }
}
