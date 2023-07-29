using System;

namespace Zametek.Utility.Logging
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.ReturnValue,
        Inherited = false,
        AllowMultiple = false)]
    public class DiagnosticLoggingAttribute
        : Attribute
    {
        public DiagnosticLoggingAttribute(LogActive logActive)
        {
            LogActive = logActive;
        }

        public LogActive LogActive
        {
            get;
        }
    }
}
