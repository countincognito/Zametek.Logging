using Castle.DynamicProxy;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Zametek.Utility.Logging
{
    public static class LogProxy
    {
        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();
        private const LogTypes c_DefaultLogTypes = LogTypes.Tracking | LogTypes.Error | LogTypes.Performance;

        public static T Create<T>(
            T instance,
            ILogger logger,
            LogTypes logTypes = c_DefaultLogTypes,
            params IInterceptor[] extraInterceptors) where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Debug.Assert(typeof(T).GetTypeInfo().IsInterface);
            List<IInterceptor> interceptors = BuildStandardInterceptors(instance, logger, logTypes);

            if (extraInterceptors != null && extraInterceptors.Any())
            {
                interceptors.AddRange(extraInterceptors);
            }

            return s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface(instance, interceptors.ToArray());
        }

        public static object Create(
            Type instanceType,
            object instance,
            ILogger logger,
            LogTypes logTypes = c_DefaultLogTypes,
            params IInterceptor[] extraInterceptors)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Debug.Assert(instanceType.GetTypeInfo().IsInterface);
            List<IInterceptor> interceptors = BuildStandardInterceptors(instance, logger, logTypes);

            if (extraInterceptors != null && extraInterceptors.Any())
            {
                interceptors.AddRange(extraInterceptors);
            }

            return s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface(instanceType, instance, interceptors.ToArray());
        }

        public static HashSet<string> FilterTheseParameters { get; } = new HashSet<string> { "password", "PASSWORD", "Password", "secret", "SECRET", "Secret" };

        private static List<IInterceptor> BuildStandardInterceptors(object instance, ILogger logger, LogTypes logTypes)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger == null)
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

            return interceptors;
        }
    }
}
