using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;

namespace Zametek.Utility.Logging.Tests
{
    [TestClass]
    public partial class InvocationEnricherTests
    {
        [TestMethod]
        public void InvocationEnricher_Enrich_PropertiesAdded()
        {
            var invocation = new TestInvocation();
            var invocationEnricher = new InvocationEnricher(invocation);
            var logEvent = new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate(new List<MessageTemplateToken>()), new List<LogEventProperty>());

            invocationEnricher.Enrich(logEvent, null);

            Assert.AreEqual(3, logEvent.Properties.Count);
            Assert.AreEqual($"\"{invocation.TargetType.Namespace}\"", logEvent.Properties[InvocationEnricher.NamespacePropertyName].ToString());
            Assert.AreEqual($"\"{invocation.TargetType.Name}\"", logEvent.Properties[InvocationEnricher.TypePropertyName].ToString());
            Assert.AreEqual($"\"{invocation.Method.Name}\"", logEvent.Properties[InvocationEnricher.MethodPropertyName].ToString());
        }
    }
}
