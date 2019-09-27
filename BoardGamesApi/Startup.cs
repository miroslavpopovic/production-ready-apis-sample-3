using System;
using BoardGamesApi.Data;
using BoardGamesApi.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace BoardGamesApi
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
            services.AddSingleton<IGamesRepository, GamesRepository>();

            services.AddJwtBearerAuthentication(Configuration);

            services.AddControllers();

            services.AddVersioning();

            services.AddOpenApi();

            services.AddHealthChecks();
            //    .AddCheck("live", check => HealthCheckResult.Healthy())
            //    .AddCheck(
            //        "random", check => new Random().Next(1, 4) switch
            //        {
            //            1 => HealthCheckResult.Healthy(),
            //            2 => HealthCheckResult.Degraded("Something is not exactly right..."),
            //            _ => HealthCheckResult.Unhealthy("We are broken :(")
            //        });
            //services.AddHealthChecksUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Inject our custom error handling middleware into ASP.NET Core pipeline
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<LimitingMiddleware>();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/live");
                //// /healthchecks-ui
                //endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                //{
                //    Predicate = _ => true,
                //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                //});
                //endpoints.MapHealthChecksUI();
            });
        }
    }
}
