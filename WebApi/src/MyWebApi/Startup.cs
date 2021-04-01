using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MyWebApi.Filters;
using MyWebApi.Infrastructure;

namespace MyWebApi
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

            services.AddControllers();
            services.AddApiVersioning();
            services.AddApiVersioning(o => 
            {
                o.AssumeDefaultVersionWhenUnspecified = true; //defaults to DefaultApiVersion
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                o.ReportApiVersions = true; //returns the supported versions in response header
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "MyWebApi", Version = "v2" });
                c.SwaggerDoc("v3", new OpenApiInfo { Title = "MyWebApi", Version = "v3" });
                c.ResolveConflictingActions(a => a.First());
                c.OperationFilter<RemoveVersionFromParameter>();
                c.DocumentFilter<ReplaceVersionWithExactValuePath>();
                c.OperationFilter<SwaggerOperationVersionHeaderFilter>();
            });
            services.AddHttpClient<IWeatherForecastService, WeatherForecastService>("metaweather", c => c.BaseAddress = new Uri(Configuration["MeteWeatherApiBaseUrl"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApi v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "MyWebApi v2");
                    c.SwaggerEndpoint("/swagger/v3/swagger.json", "MyWebApi v3");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
