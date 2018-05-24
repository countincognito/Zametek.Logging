using Castle.DynamicProxy;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging
{
    public class AsyncDiagnosticLoggingInterceptor
        : ProcessingAsyncInterceptor<DiagnosticLogState>
    {
        public const string LogTypeName = nameof(LogType);
        public const string ArgumentsName = nameof(IInvocation.Arguments);
        public const string ReturnValueName = nameof(IInvocation.ReturnValue);
        public const string VoidSubstitute = @"__VOID__";
        public const string FilteredParameterSubstitute = @"__FILTERED__";
        private readonly ILogger m_Logger;
        private readonly HashSet<string> m_FilterTheseParameters;

        public AsyncDiagnosticLoggingInterceptor(ILogger logger)
            : this(logger, new HashSet<string>())
        {
        }

        public AsyncDiagnosticLoggingInterceptor(ILogger logger, HashSet<string> filterTheseParameters)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_FilterTheseParameters = filterTheseParameters ?? throw new ArgumentNullException(nameof(filterTheseParameters));
        }

        protected override DiagnosticLogState StartingInvocation(IInvocation invocation)
        {
            Debug.Assert(invocation != null);
            Debug.Assert(invocation.TargetType != null);

            LogActive classActiveState = LogActive.Off;

            // Check for DiagnosticLogging Class scope.
            var classDiagnosticAttribute = invocation
                .TargetType
                .GetCustomAttributes(typeof(DiagnosticLoggingAttribute), false)
                .FirstOrDefault() as DiagnosticLoggingAttribute;

            if (classDiagnosticAttribute != null)
            {
                classActiveState = classDiagnosticAttribute.LogActive;
            }

            LogActive methodActiveState = LogMethodBeforeInvocation(invocation, classActiveState);
            return new DiagnosticLogState(methodActiveState);
        }

        protected override void CompletedInvocation(IInvocation invocation, DiagnosticLogState activeState, object returnValue)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            LogActive classActiveState = activeState;
            LogMethodAfterInvocation(invocation, classActiveState, returnValue);
        }

        private LogActive LogMethodBeforeInvocation(IInvocation invocation, LogActive activeState)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            LogActive methodActiveState = activeState;
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            Debug.Assert(methodInfo != null);

            // Check for DiagnosticLogging Method scope.
            var methodDiagnosticAttribute = methodInfo
                .GetCustomAttribute(typeof(DiagnosticLoggingAttribute), false) as DiagnosticLoggingAttribute;

            if (methodDiagnosticAttribute != null)
            {
                methodActiveState = methodDiagnosticAttribute.LogActive;
            }

            (IList<object> filteredParameters, LogActive anyParametersToLog) = FilterParameters(invocation, methodInfo, methodActiveState, m_FilterTheseParameters);

            if (anyParametersToLog == LogActive.On)
            {
                using (LogContext.PushProperty(LogTypeName, LogType.Diagnostic))
                using (LogContext.Push(new InvocationEnricher(invocation)))
                using (LogContext.PushProperty(ArgumentsName, filteredParameters, destructureObjects: true))
                {
                    string logMessage = $"{GetSourceMessage(invocation)} invocation started";
                    m_Logger.Information(logMessage);
                }
            }

            return methodActiveState;
        }

        private static (IList<object>, LogActive) FilterParameters(
            IInvocation invocation,
            MethodInfo methodInfo,
            LogActive activeState,
            HashSet<string> filterTheseParameters)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            if (filterTheseParameters == null)
            {
                throw new ArgumentNullException(nameof(filterTheseParameters));
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            Debug.Assert(parameterInfos != null);

            object[] parameters = invocation.Arguments;
            Debug.Assert(parameters != null);

            Debug.Assert(parameterInfos.Length == parameters.Length);

            var filteredParameters = new List<object>();

            // Send a message back whether anything should be logged.
            LogActive anyParametersToLog = activeState;

            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                LogActive parameterActiveState = activeState;
                ParameterInfo parameterInfo = parameterInfos[parameterIndex];

                // Check if the parameter name matches any of the pre-determined filters.
                if (filterTheseParameters.Contains(parameterInfo.Name))
                {
                    parameterActiveState = LogActive.Off;
                }

                // Check for DiagnosticLogging Parameter scope.
                var parameterDiagnosticAttribute = parameterInfo
                    .GetCustomAttribute(typeof(DiagnosticLoggingAttribute), false) as DiagnosticLoggingAttribute;

                if (parameterDiagnosticAttribute != null)
                {
                    parameterActiveState = parameterDiagnosticAttribute.LogActive;
                }

                object parameterValue = FilteredParameterSubstitute;

                if (parameterActiveState == LogActive.On)
                {
                    anyParametersToLog = LogActive.On;
                    parameterValue = parameters[parameterIndex];
                }

                filteredParameters.Add(parameterValue);
            }

            return (filteredParameters, anyParametersToLog);
        }

        private void LogMethodAfterInvocation(IInvocation invocation, LogActive activeState, object returnValue)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            LogActive methodActiveState = activeState;
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            Debug.Assert(methodInfo != null);

            (object filteredReturnValue, LogActive anyParametersToLog) = FilterReturnValue(methodInfo, methodActiveState, returnValue);

            if (anyParametersToLog == LogActive.On)
            {
                using (LogContext.PushProperty(LogTypeName, LogType.Diagnostic))
                using (LogContext.Push(new InvocationEnricher(invocation)))
                using (LogContext.PushProperty(ReturnValueName, filteredReturnValue, destructureObjects: true))
                {
                    string logMessage = $"{GetSourceMessage(invocation)} invocation ended";
                    m_Logger.Information(logMessage);
                }
            }
        }

        private static (object, LogActive) FilterReturnValue(MethodInfo methodInfo, LogActive activeState, object returnValue)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            LogActive returnParameterActiveState = activeState;
            ParameterInfo parameterInfo = methodInfo.ReturnParameter;
            Debug.Assert(parameterInfo != null);

            // Check for DiagnosticLogging ReturnValue scope.
            var returnValueDiagnosticAttribute = parameterInfo
                .GetCustomAttribute(typeof(DiagnosticLoggingAttribute), false) as DiagnosticLoggingAttribute;

            if (returnValueDiagnosticAttribute != null)
            {
                returnParameterActiveState = returnValueDiagnosticAttribute.LogActive;
            }

            object returnParameterValue = FilteredParameterSubstitute;

            // Send a message back whether anything should be logged.
            LogActive anyParametersToLog = activeState;

            if (returnParameterActiveState == LogActive.On)
            {
                anyParametersToLog = LogActive.On;

                if (parameterInfo.ParameterType == typeof(void)
                    || parameterInfo.ParameterType == typeof(Task))
                {
                    returnParameterValue = VoidSubstitute;
                }
                else
                {
                    returnParameterValue = returnValue;
                }
            }

            return (returnParameterValue, anyParametersToLog);
        }

        private static string GetSourceMessage(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            return $"diagnostic-{invocation.TargetType?.Namespace}.{invocation.TargetType?.Name}.{invocation.Method?.Name}";
        }
    }

    public class DiagnosticLogState
    {
        public DiagnosticLogState(LogActive activeState)
        {
            ActiveState = activeState;
        }

        public LogActive ActiveState
        {
            get;
        }

        public static implicit operator LogActive(DiagnosticLogState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            return state.ActiveState;
        }
    }
}
