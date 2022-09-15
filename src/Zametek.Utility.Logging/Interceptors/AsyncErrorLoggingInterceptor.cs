using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using Serilog.Exceptions.Core;
using System;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging
{
    public class AsyncErrorLoggingInterceptor
        : AsyncInterceptorBase
    {
        public const string LogTypesName = nameof(LogTypes);
        private readonly ILogger m_Logger;
        private readonly IDestructuringOptions m_DestructuringOptions;

        public AsyncErrorLoggingInterceptor(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_DestructuringOptions = new DestructuringOptionsBuilder().WithDefaultDestructurers();
        }

        protected override async Task InterceptAsync(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceedInfo is null)
            {
                throw new ArgumentNullException(nameof(proceedInfo));
            }
            if (proceed is null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            try
            {
                await proceed(invocation, proceedInfo).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogException(invocation, ex);
                throw;
            }
        }

        protected override async Task<T> InterceptAsync<T>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<T>> proceed)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceedInfo is null)
            {
                throw new ArgumentNullException(nameof(proceedInfo));
            }
            if (proceed is null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            try
            {
                return await proceed(invocation, proceedInfo).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogException(invocation, ex);
                throw;
            }
        }

        private void LogException(
            IInvocation invocation,
            Exception ex)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            using (LogContext.PushProperty(LogTypesName, LogTypes.Error))
            using (LogContext.Push(new InvocationEnricher(invocation)))
            using (LogContext.Push(new ExceptionEnricher(m_DestructuringOptions)))
            {
                m_Logger.Error(ex, $"{GetSourceMessage(invocation)}");
            }
        }

        private static string GetSourceMessage(IInvocation invocation)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            return $"error-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }
}
