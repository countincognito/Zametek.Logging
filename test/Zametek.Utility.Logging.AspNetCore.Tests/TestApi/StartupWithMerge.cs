using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Zametek.Utility.Logging.AspNetCore.Tests
{
    public class StartupWithMerge
        : StartupBase
    {
        public StartupWithMerge(IConfiguration configuration)
            : base(configuration)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            // Use this to add unique callchain IDs to each call into a controller, and add custom headers.
            app.UseTrackingContextMiddleware(TrackingHeadersGenerator);
            app.UseMergeTrackingContextMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
