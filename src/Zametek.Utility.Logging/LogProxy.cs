using Castle.DynamicProxy;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zametek.Utility.Logging
{
    public static class LogProxy
    {
        #region Fields

        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();
        internal const LogTypes DefaultLogTypes = LogTypes.Tracking | LogTypes.Error | LogTypes.Performance | LogTypes.Invocation;

        #endregion

        #region Ctors

        static LogProxy()
        {
            FilterTheseParameters = new HashSet<string> { "password", "PASSWORD", "Password", "secret", "SECRET", "Secret" };
        }

        #endregion

        #region Properties

        public static HashSet<string> FilterTheseParameters
        {
            get;
            private set;
        }

        #endregion

        #region Public Members

        public static T Create<T>(
            T instance,
            ILogger logger,
            LogTypes logTypes = DefaultLogTypes,
            params IInterceptor[] extraInterceptors) where T : class
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            instance.ThrowIfNotInterface();

            List<IInterceptor> interceptors = BuildStandardInterceptors(logger, logTypes);

            if (extraInterceptors != null && extraInterceptors.Any())
            {
                interceptors.AddRange(extraInterceptors);
            }

            return s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface(instance, interceptors.ToArray());
        }

        public static object Create(
            Type interfaceType,
            object instance,
            ILogger logger,
            LogTypes logTypes = DefaultLogTypes,
            params IInterceptor[] extraInterceptors)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            interfaceType.ThrowIfNotInterface();

            List<IInterceptor> interceptors = BuildStandardInterceptors(logger, logTypes);

            if (extraInterceptors != null && extraInterceptors.Any())
            {
                interceptors.AddRange(extraInterceptors);
            }

            return s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface(interfaceType, instance, interceptors.ToArray());
        }

        #endregion

        #region Private Members

        private static List<IInterceptor> BuildStandardInterceptors(
            ILogger logger,
            LogTypes logTypes)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var interceptors = new List<IInterceptor>();

            if (logTypes.HasFlag(LogTypes.Tracking))
            {
                interceptors.Add(new AsyncTrackingInterceptor().ToInterceptor());
            }

            if (logTypes.HasFlag(LogTypes.Error))
            {
                interceptors.Add(new AsyncErrorLoggingInterceptor(logger).ToInterceptor());
            }

            if (logTypes.HasFlag(LogTypes.Performance))
            {
                interceptors.Add(new AsyncPerformanceLoggingInterceptor(logger).ToInterceptor());
            }

            if (logTypes.HasFlag(LogTypes.Diagnostic))
            {
                interceptors.Add(new AsyncDiagnosticLoggingInterceptor(logger, FilterTheseParameters).ToInterceptor());
            }

            if (logTypes.HasFlag(LogTypes.Invocation))
            {
                interceptors.Add(new AsyncInvocationLoggingInterceptor(logger).ToInterceptor());
            }

            return interceptors;
        }

        #endregion
    }
}
