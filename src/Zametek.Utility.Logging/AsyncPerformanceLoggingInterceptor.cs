using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using System;
using System.Diagnostics;

namespace Zametek.Utility.Logging
{
    public class AsyncPerformanceLoggingInterceptor
        : AsyncTimingInterceptor
    {
        public const string LogTypesName = nameof(LogTypes);
        private readonly ILogger m_Logger;

        public AsyncPerformanceLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void StartingTiming(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            using (LogContext.PushProperty(LogTypesName, LogTypes.Performance))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} invocation started");
            }
        }

        protected override void CompletedTiming(IInvocation invocation, Stopwatch state)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            long elapsedMilliseconds = state.ElapsedMilliseconds;

            using (LogContext.PushProperty(LogTypesName, LogTypes.Performance))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} invocation ended, ElapsedMilliseconds: {{ElapsedMilliseconds}}", elapsedMilliseconds);
            }
        }

        private static string GetSourceMessage(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            return $"performance-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }
}
