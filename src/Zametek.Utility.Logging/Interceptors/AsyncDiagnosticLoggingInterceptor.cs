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
        public const string LogTypesName = nameof(LogTypes);
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

        public AsyncDiagnosticLoggingInterceptor(
            ILogger logger,
            HashSet<string> filterTheseParameters)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_FilterTheseParameters = filterTheseParameters ?? throw new ArgumentNullException(nameof(filterTheseParameters));
        }

        protected override DiagnosticLogState StartingInvocation(IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            Debug.Assert(invocation.TargetType != null);

            LogActive classActiveState = LogActive.Off;

            // Check for DiagnosticLogging Class scope.

            if (invocation
                .TargetType.GetTypeInfo()
                .GetCustomAttributes(typeof(DiagnosticLoggingAttribute), false)
                .FirstOrDefault() is DiagnosticLoggingAttribute classDiagnosticAttribute)
            {
                classActiveState = classDiagnosticAttribute.LogActive;
            }

            LogActive methodActiveState = LogMethodBeforeInvocation(invocation, classActiveState);
            return new DiagnosticLogState(methodActiveState);
        }

        protected override void CompletedInvocation(
            IInvocation invocation,
            DiagnosticLogState activeState,
            object returnValue)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (activeState == null)
            {
                throw new ArgumentNullException(nameof(activeState));
            }

            LogActive classActiveState = activeState.ActiveState;
            LogMethodAfterInvocation(invocation, classActiveState, returnValue);
        }

        private LogActive LogMethodBeforeInvocation(
            IInvocation invocation,
            LogActive activeState)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            LogActive methodActiveState = activeState;
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            Debug.Assert(methodInfo != null);

            // Check for DiagnosticLogging Method scope.

            if (methodInfo
                .GetCustomAttribute(typeof(DiagnosticLoggingAttribute), false) is DiagnosticLoggingAttribute methodDiagnosticAttribute)
            {
                methodActiveState = methodDiagnosticAttribute.LogActive;
            }

            Tuple<IList<object>, LogActive> filteredParametersTuple = FilterParameters(invocation, methodInfo, methodActiveState, m_FilterTheseParameters);

            IList<object> filteredParameters = filteredParametersTuple.Item1;
            LogActive anyParametersToLog = filteredParametersTuple.Item2;

            if (anyParametersToLog == LogActive.On)
            {
                using (LogContext.PushProperty(LogTypesName, LogTypes.Diagnostic))
                using (LogContext.Push(new InvocationEnricher(invocation)))
                using (LogContext.PushProperty(ArgumentsName, filteredParameters, destructureObjects: true))
                {
                    string logMessage = $"{GetSourceMessage(invocation)} invocation started";
                    m_Logger.Information(logMessage);
                }
            }

            return methodActiveState;
        }

        private static Tuple<IList<object>, LogActive> FilterParameters(
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

            IList<object> filteredParameters = new List<object>();

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

                if (parameterInfo
                    .GetCustomAttribute(typeof(DiagnosticLoggingAttribute), false) is DiagnosticLoggingAttribute parameterDiagnosticAttribute)
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

            return Tuple.Create(filteredParameters, anyParametersToLog);
        }

        private void LogMethodAfterInvocation(
            IInvocation invocation,
            LogActive activeState,
            object returnValue)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            LogActive methodActiveState = activeState;
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            Debug.Assert(methodInfo != null);

            Tuple<object, LogActive> filteredReturnValueTuple = FilterReturnValue(methodInfo, methodActiveState, returnValue);

            object filteredReturnValue = filteredReturnValueTuple.Item1;
            LogActive anyParametersToLog = filteredReturnValueTuple.Item2;

            if (anyParametersToLog == LogActive.On)
            {
                using (LogContext.PushProperty(LogTypesName, LogTypes.Diagnostic))
                using (LogContext.Push(new InvocationEnricher(invocation)))
                using (LogContext.PushProperty(ReturnValueName, filteredReturnValue, destructureObjects: true))
                {
                    string logMessage = $"{GetSourceMessage(invocation)} invocation ended";
                    m_Logger.Information(logMessage);
                }
            }
        }

        private static Tuple<object, LogActive> FilterReturnValue(
            MethodInfo methodInfo,
            LogActive activeState,
            object returnValue)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            LogActive returnParameterActiveState = activeState;
            ParameterInfo parameterInfo = methodInfo.ReturnParameter;
            Debug.Assert(parameterInfo != null);

            // Check for DiagnosticLogging ReturnValue scope.

            if (parameterInfo
                .GetCustomAttribute(typeof(DiagnosticLoggingAttribute), false) is DiagnosticLoggingAttribute returnValueDiagnosticAttribute)
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

            return Tuple.Create(returnParameterValue, anyParametersToLog);
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
    }
}
