using Serilog.Events;
using Serilog.Parsing;
using Shouldly;
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

            logEvent.Properties.Count.ShouldBe(3);
            logEvent.Properties[InvocationEnricher.NamespacePropertyName].ToString().ShouldBe($@"""{invocation.TargetType.Namespace}""");
            logEvent.Properties[InvocationEnricher.TypePropertyName].ToString().ShouldBe($@"""{invocation.TargetType.Name}""");
            logEvent.Properties[InvocationEnricher.MethodPropertyName].ToString().ShouldBe($@"""{invocation.Method.Name}""");
        }
    }
}
