using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Zametek.Utility.Logging
{
    public class TrackingContextEnricher
        : ILogEventEnricher
    {
        /// <summary>
        /// The property name added to enriched log events.
        /// </summary>
        public const string CallChainIdPropertyName = nameof(TrackingContext.CallChainId);
        public const string OriginatorUtcTimestampPropertyName = nameof(TrackingContext.OriginatorUtcTimestamp);

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            TrackingContext context = TrackingContext.Current;
            if (context != null)
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty(CallChainIdPropertyName, new ScalarValue(context.CallChainId.ToString())));
                logEvent.AddPropertyIfAbsent(new LogEventProperty(OriginatorUtcTimestampPropertyName, new ScalarValue(context.OriginatorUtcTimestamp.ToString("o", CultureInfo.InvariantCulture))));

                foreach (KeyValuePair<string, string> kvp in context.ExtraHeaders.OrderBy(x => x.Key, StringComparer.Ordinal))
                {
                    logEvent.AddPropertyIfAbsent(new LogEventProperty(kvp.Key, new ScalarValue(kvp.Value)));
                }
            }
        }
    }
}
