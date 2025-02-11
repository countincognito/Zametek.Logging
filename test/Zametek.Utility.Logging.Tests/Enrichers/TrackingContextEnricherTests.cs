using Serilog.Events;
using Serilog.Parsing;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Zametek.Utility.Logging.Tests
{
    public class TrackingContextEnricherTests
    {
        [Fact]
        public void TrackingContextEnricher_GivenNoTrackingContext_WhenEnricherApplied_ThenNoPropertiesAdded()
        {
            TrackingContext currentTrackingContext = TrackingContext.Current;

            currentTrackingContext.ShouldBeNull();

            var trackingContextEnricher = new TrackingContextEnricher();
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            trackingContextEnricher.Enrich(logEvent, null);

            logEvent.Properties.ShouldBeEmpty();
        }

        [Fact]
        public void TrackingContextEnricher_GivenTrackingContext_WhenEnricherApplied_ThenPropertiesAdded()
        {
            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            currentTrackingContext.ShouldNotBeNull();

            var trackingContextEnricher = new TrackingContextEnricher();
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            trackingContextEnricher.Enrich(logEvent, null);

            logEvent.Properties.Count.ShouldBe(2);
            logEvent.Properties[TrackingContextEnricher.CallChainIdPropertyName].ToString().ShouldBe($"\"{currentTrackingContext.CallChainId}\"");
            logEvent.Properties[TrackingContextEnricher.OriginatorUtcTimestampPropertyName].ToString().ShouldBe($"\"{currentTrackingContext.OriginatorUtcTimestamp.ToString("o")}\"");
        }

        [Fact]
        public void TrackingContextEnricher_GivenTrackingContextWithExtraHeaders_WhenEnricherApplied_ThenPropertiesAdded()
        {
            var extraHeaders = new Dictionary<string, string> {
                { "FirstKey", "FirstValue" },
                { "SecondKey", "SecondValue" }
            };

            TrackingContext.NewCurrent(extraHeaders);
            TrackingContext currentTrackingContext = TrackingContext.Current;

            currentTrackingContext.ShouldNotBeNull();

            var trackingContextEnricher = new TrackingContextEnricher();
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            trackingContextEnricher.Enrich(logEvent, null);

            logEvent.Properties.Count.ShouldBe(4);
            logEvent.Properties[TrackingContextEnricher.CallChainIdPropertyName].ToString().ShouldBe($@"""{currentTrackingContext.CallChainId}""");
            logEvent.Properties[TrackingContextEnricher.OriginatorUtcTimestampPropertyName].ToString().ShouldBe($@"""{currentTrackingContext.OriginatorUtcTimestamp.ToString("o")}""");
            logEvent.Properties["FirstKey"].ToString().ShouldBe(@"""FirstValue""");
            logEvent.Properties["SecondKey"].ToString().ShouldBe(@"""SecondValue""");
        }
    }
}
