using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;

namespace Zametek.Utility.Logging.Tests
{
    [TestClass]
    public partial class TrackingContextEnricherTests
    {
        [TestMethod]
        public void TrackingContextEnricher_EnrichWithoutContext_PropertiesAdded()
        {
            TrackingContext currentTrackingContext = TrackingContext.Current;

            Assert.IsNull(currentTrackingContext);

            var trackingContextEnricher = new TrackingContextEnricher();
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            trackingContextEnricher.Enrich(logEvent, null);

            Assert.AreEqual(0, logEvent.Properties.Count);
        }

        [TestMethod]
        public void TrackingContextEnricher_EnrichWithContext_PropertiesAdded()
        {
            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            Assert.IsNotNull(currentTrackingContext);

            var trackingContextEnricher = new TrackingContextEnricher();
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            trackingContextEnricher.Enrich(logEvent, null);

            Assert.AreEqual(2, logEvent.Properties.Count);
            Assert.AreEqual($"\"{currentTrackingContext.CallChainId}\"", logEvent.Properties[TrackingContextEnricher.CallChainIdPropertyName].ToString());
            Assert.AreEqual($"\"{currentTrackingContext.OriginatorUtcTimestamp.ToString("o")}\"", logEvent.Properties[TrackingContextEnricher.OriginatorUtcTimestampPropertyName].ToString());
        }

        [TestMethod]
        public void TrackingContextEnricher_EnrichWithContextAndExtraHeaders_PropertiesAdded()
        {
            var extraHeaders = new Dictionary<string, string> {
                { "FirstKey", "FirstValue" },
                { "SecondKey", "SecondValue" }
            };

            TrackingContext.NewCurrent(extraHeaders);
            TrackingContext currentTrackingContext = TrackingContext.Current;

            Assert.IsNotNull(currentTrackingContext);

            var trackingContextEnricher = new TrackingContextEnricher();
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            trackingContextEnricher.Enrich(logEvent, null);

            Assert.AreEqual(4, logEvent.Properties.Count);
            Assert.AreEqual($"\"{currentTrackingContext.CallChainId}\"", logEvent.Properties[TrackingContextEnricher.CallChainIdPropertyName].ToString());
            Assert.AreEqual($"\"{currentTrackingContext.OriginatorUtcTimestamp.ToString("o")}\"", logEvent.Properties[TrackingContextEnricher.OriginatorUtcTimestampPropertyName].ToString());
            Assert.AreEqual("\"FirstValue\"", logEvent.Properties["FirstKey"].ToString());
            Assert.AreEqual("\"SecondValue\"", logEvent.Properties["SecondKey"].ToString());
        }
    }
}
