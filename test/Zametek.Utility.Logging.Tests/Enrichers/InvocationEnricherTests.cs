using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Zametek.Utility.Logging.Tests
{
    public partial class InvocationEnricherTests
    {
        [Fact]
        public void InvocationEnricher_GivenTestInvocation_WhenEnricherApplied_ThenPropertiesAdded()
        {
            var invocation = new TestInvocation();
            var invocationEnricher = new InvocationEnricher(invocation);
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            invocationEnricher.Enrich(logEvent, null);

            logEvent.Properties.Count.Should().Be(3);
            logEvent.Properties[InvocationEnricher.NamespacePropertyName].ToString().Should().Be($@"""{invocation.TargetType.Namespace}""");
            logEvent.Properties[InvocationEnricher.TypePropertyName].ToString().Should().Be($@"""{invocation.TargetType.Name}""");
            logEvent.Properties[InvocationEnricher.MethodPropertyName].ToString().Should().Be($@"""{invocation.Method.Name}""");
        }
    }
}
