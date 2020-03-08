using Destructurama;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Zametek.Utility.Logging.AspNetCore.Tests
{
    public abstract class StartupBase
    {
        public const string TraceIdentifierName = nameof(HttpContext.TraceIdentifier);
        public const string CountryOfOriginName = @"CountryOfOrigin";
        public const string RandomStringGeneratedWitEachCallName = @"RandomStringGeneratedWitEachCall";

        public Func<HttpContext, IDictionary<string, string>> TrackingHeadersGenerator =
            (context) => new Dictionary<string, string>()
            {
                { TraceIdentifierName, context.TraceIdentifier },
                { CountryOfOriginName, "UK" },
                { RandomStringGeneratedWitEachCallName, Guid.NewGuid().ToDashedString() },
            };

        public StartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //public void ConfigureServices(IServiceCollection services)
        //{

        //}

        public static void ConfigureServicesWithLogSink(
            IServiceCollection services,
            TextWriter textWriter,
            string outputTemplate)
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .Destructure.ByIgnoringProperties<ResponseDto>(x => x.Password)
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.TextWriter(textWriter, outputTemplate: outputTemplate)
                .CreateLogger();
            Log.Logger = serilog;

            services.ActivateLogTypes(LogTypes.All);

            services.AddSingleton(serilog);
            services.TryAddSingletonWithLogProxy<IValueAccess, ValueAccess>();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc();

            services.AddControllers();
        }
    }
}
