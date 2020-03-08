using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging
{
    public class AsyncInvocationLoggingInterceptor
        : AsyncInterceptorBase
    {
        public const string LogTypesName = nameof(LogTypes);
        private readonly ILogger m_Logger;

        public AsyncInvocationLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task InterceptAsync(
            IInvocation invocation,
            Func<IInvocation, Task> proceed)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceed == null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            using (LogContext.PushProperty(LogTypesName, LogTypes.Invocation))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} started");
            }

            await proceed(invocation).ConfigureAwait(false);

            using (LogContext.PushProperty(LogTypesName, LogTypes.Invocation))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} ended");
            }
        }

        protected override async Task<TResult> InterceptAsync<TResult>(
            IInvocation invocation,
            Func<IInvocation, Task<TResult>> proceed)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceed == null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            using (LogContext.PushProperty(LogTypesName, LogTypes.Invocation))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} started");
            }

            var result = await proceed(invocation).ConfigureAwait(false);

            using (LogContext.PushProperty(LogTypesName, LogTypes.Invocation))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            {
                m_Logger.Information($"{GetSourceMessage(invocation)} ended");
            }

            return result;
        }

        private static string GetSourceMessage(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            return $"invocation-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }
}
