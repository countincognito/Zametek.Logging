using System;

namespace Zametek.Utility.Logging
{
    [Flags]
    public enum LogTypes
    {
        None = 0,
        Tracking = 1 << 0,
        Error = 1 << 1,
        Performance = 1 << 2,
        Diagnostic = 1 << 3,
        All = Tracking | Error | Performance | Diagnostic,
    }
}
