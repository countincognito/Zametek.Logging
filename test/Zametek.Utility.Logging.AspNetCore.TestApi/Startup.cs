using Destructurama;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace Zametek.Utility.Logging.AspNetCore.TestApi
{
    public class Startup
    {
        public const string RemoteIpAddressName = nameof(ConnectionInfo.RemoteIpAddress);
        public const string TraceIdentifierName = nameof(HttpContext.TraceIdentifier);
        public const string UserIdName = @"UserId";
        public const string ConnectionIdName = @"ConnectionId";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Use FromLoggingProxy to enrich the serilog output with diagnostic, tracking, error and performance logging.
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .Destructure.ByIgnoringProperties<ResponseDto>(x => x.Password)
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
            Log.Logger = serilog;

            //LogProxy.FilterTheseParameters.Add("requestDto");

            // Wrapping a class in a LogProxy automatically enriches the serilog output.
            var valueAccess = LogProxy.Create<IValueAccess>(new ValueAccess(serilog), serilog, LogType.All);

            services.AddSingleton(valueAccess);
            services.AddSingleton(serilog);

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use this to add unique callchain IDs to each call into a controller, and add custom headers.
            app.UseTrackingMiddleware(
                (context) => new Dictionary<string, string>()
                {
                    { RemoteIpAddressName, context.Connection?.RemoteIpAddress?.ToString() },
                    { TraceIdentifierName, context.TraceIdentifier },
                    { UserIdName, context.User?.Identity?.Name },
                    { ConnectionIdName, context.Connection.Id },
                    { "Country of origin", "UK" },
                    { "Random string generated with each call", System.Guid.NewGuid().ToString() }
                });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test - API");
            });
        }
    }
}
