using Castle.DynamicProxy;

namespace Zametek.Utility.Logging.Tests
{
    public partial class AsyncDiagnosticLoggingInterceptorTests
    {
        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();
        private static readonly string s_FirstParam = @"MyFirstParam";
        private static readonly string s_SecondParam = @"MySecondParam";
        private static readonly string s_ParamsLogReturn = $@"[""{s_FirstParam}"", ""{s_SecondParam}""]";
        private static readonly string s_NoParamsLogReturn = @"[]";
        private static readonly string s_FilteredParamsLogReturn = $@"[""{AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute}"", ""{AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute}""]";
    }
}
