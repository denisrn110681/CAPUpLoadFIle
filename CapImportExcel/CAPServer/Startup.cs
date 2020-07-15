using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CAPServer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace CAPServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
            services.AddScoped<DataContext, DataContext>();
            services.AddCors(o => o.AddPolicy("CorePolicy", builder =>
            {
                builder.AllowAnyMethod();
            }));
            services.AddCors(options =>
            {
                
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                builder =>
                                {
                                    //builder.AllowAnyMethod();
                                    builder.WithOrigins("*").AllowAnyHeader();
                                    builder.WithOrigins("http://localhost:4200");
                                }
                                );
            }
                           );

            services.AddControllers();
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
            // app.UseCors(option => option.AllowAnyMethod());
            // app.UseCors(option => option.AllowAnyOrigin());
            // app.UseCors(option => option.AllowAnyHeader());
           
            app.UseCors(MyAllowSpecificOrigins);
             app.UseCors("CorePolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
