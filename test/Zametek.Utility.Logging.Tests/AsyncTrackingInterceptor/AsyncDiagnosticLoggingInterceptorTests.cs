using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zametek.Utility.Logging.Tests
{
    [TestClass]
    public partial class AsyncDiagnosticLoggingInterceptorTests
    {
        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();
        private const string m_FirstParam = "MyFirstParam";
        private const string m_SecondParam = "MySecondParam";
        private const string m_ParamsLogReturn = "[\"" + m_FirstParam + "\", \"" + m_SecondParam + "\"]";
        private const string m_NoParamsLogReturn = "[]";
        private const string m_FilteredParamsLogReturn = "[\"" + AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute + "\", \"" + AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute + "\"]";
    }
}
