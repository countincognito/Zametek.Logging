using System;

namespace Zametek.Utility.Logging
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = false)]
    public class NoDiagnosticLoggingAttribute
        : Attribute
    {
    }
}
