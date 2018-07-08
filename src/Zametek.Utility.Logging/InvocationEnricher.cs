﻿using Castle.DynamicProxy;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics;

namespace Zametek.Utility.Logging
{
    public class InvocationEnricher
        : ILogEventEnricher
    {
        /// <summary>
        /// The property name added to enriched log events.
        /// </summary>
        public const string NamespacePropertyName = nameof(Type.Namespace);
        public const string TypePropertyName = nameof(Type);
        public const string MethodPropertyName = @"Method";

        private readonly IInvocation m_Invocation;

        public InvocationEnricher(IInvocation invocation)
        {
            m_Invocation = invocation ?? throw new ArgumentNullException(nameof(invocation));
        }

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            Debug.Assert(logEvent != null);
            Debug.Assert(m_Invocation.TargetType != null);
            Debug.Assert(m_Invocation.Method != null);

            logEvent.AddPropertyIfAbsent(new LogEventProperty(NamespacePropertyName, new ScalarValue(m_Invocation.TargetType?.Namespace)));
            logEvent.AddPropertyIfAbsent(new LogEventProperty(TypePropertyName, new ScalarValue(m_Invocation.TargetType?.Name)));
            logEvent.AddPropertyIfAbsent(new LogEventProperty(MethodPropertyName, new ScalarValue(m_Invocation.Method?.Name)));
        }
    }
}
