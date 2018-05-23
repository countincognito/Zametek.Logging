using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.IO;

namespace Zametek.Utility.Logging.Tests
{
    [TestClass]
    public class AsyncDiagnosticLoggingInterceptorTests
    {
        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();
        private const string m_FirstParam = "MyFirstParam";
        private const string m_SecondParam = "MySecondParam";
        private const string m_ParamLogReturn = "[\"" + m_FirstParam + "\", \"" + m_SecondParam + "\"]";

        private static ITestDiagnosticLoggingService CreateProxy(StringWriter returnOutput, StringWriter paramsOutput)
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .WriteTo.TextWriter(returnOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ReturnValueName}}}")
                .WriteTo.TextWriter(paramsOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ArgumentsName}}}")
                .CreateLogger();

            var instance = new TestDiagnosticLoggingService();
            var interceptor = new AsyncDiagnosticLoggingInterceptor(serilog);

            ITestDiagnosticLoggingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestDiagnosticLoggingService>(instance, interceptor.ToInterceptor());
            return proxy;
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_NoParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            proxy.NoParamsReturnVoid();

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }


        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_SomeParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_NoParamsReturnString_OutputEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            string returnValue = proxy.NoParamsReturnString();

            Assert.AreEqual(TestDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_SomeParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_SomeParamsReturnVoidActiveParamsActiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_SomeParamsReturnStringActiveParamsActiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_SomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_SomeParamsReturnStringInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }
    }
}
