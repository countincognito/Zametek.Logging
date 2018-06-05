using Castle.DynamicProxy;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zametek.Utility.Logging
{
    public class LogProxy
    {
        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();
        private const LogType c_DefaultLogType = LogType.Tracking | LogType.Error | LogType.Performance;

        public static I Create<I>(
            I instance,
            ILogger logger,
            LogType logType = c_DefaultLogType,
            params IInterceptor[] extraInterceptors) where I : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Debug.Assert(typeof(I).IsInterface);
            List<IInterceptor> interceptors = BuildStandardInterceptors(instance, logger, logType);

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
            LogType logType = c_DefaultLogType,
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

            Debug.Assert(instanceType.IsInterface);
            List<IInterceptor> interceptors = BuildStandardInterceptors(instance, logger, logType);

            if (extraInterceptors != null && extraInterceptors.Any())
            {
                interceptors.AddRange(extraInterceptors);
            }

            return s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface(instanceType, instance, interceptors.ToArray());
        }

        public static HashSet<string> FilterTheseParameters { get; } = new HashSet<string> { "password", "PASSWORD", "Password", "secret", "SECRET", "Secret" };

        private static List<IInterceptor> BuildStandardInterceptors(object instance, ILogger logger, LogType logType)
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

            if (logType.HasFlag(LogType.Tracking))
            {
                interceptors.Add(new AsyncTrackingInterceptor().ToInterceptor());
            }

            if (logType.HasFlag(LogType.Error))
            {
                interceptors.Add(new AsyncErrorLoggingInterceptor(logger).ToInterceptor());
            }

            if (logType.HasFlag(LogType.Performance))
            {
                interceptors.Add(new AsyncPerformanceLoggingInterceptor(logger).ToInterceptor());
            }

            if (logType.HasFlag(LogType.Diagnostic))
            {
                interceptors.Add(new AsyncDiagnosticLoggingInterceptor(logger, FilterTheseParameters).ToInterceptor());
            }

            return interceptors;
        }
    }
}
