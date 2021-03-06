using CityOfTulsaUI.Classes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityOfTulsa.UI {

   public class Startup {

      public IConfiguration Configuration { get; }

      public Startup(IConfiguration config) {
         this.Configuration = config;
      }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services) {

         services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()
            .AddNewtonsoftJson()
            ;

         services
            .AddMvc()
            .AddSessionStateTempDataProvider()
            ;
            
         services.AddSession();

         services.AddControllers();

         //AppSettings appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
         //services.AddSingleton(appSettings);
         services.Configure<AppSettings>(this.Configuration.GetSection("AppSettings"));
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

         if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
         }
         else {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
         }

         app.UseHttpsRedirection();

         app.UseStaticFiles();

         app.UseRouting();

         app.UseAuthorization();

         app.UseSession();

         //app.UseMvcWithDefaultRoute();

         app.UseEndpoints(endpoints => {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
         });
      }
   }
}
