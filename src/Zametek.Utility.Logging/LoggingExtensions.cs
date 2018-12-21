using Destructurama;
using Serilog;
using Serilog.Configuration;
using System;

namespace Zametek.Utility.Logging
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration FromTrackingContext(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            return enrichmentConfiguration
                .With<TrackingContextEnricher>()
                .Enrich.WithMachineName();
        }

        public static LoggerConfiguration FromLogProxy(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            return enrichmentConfiguration.FromLogContext()
                .Destructure.UsingAttributes()
                .Enrich.FromTrackingContext();
        }
    }
}
