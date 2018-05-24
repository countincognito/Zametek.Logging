using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace Zametek.Utility.Logging.Tests
{
    public partial class AsyncDiagnosticLoggingInterceptorTests
    {
        private static ITestDiagnosticLoggingService CreateBareClassProxy(StringWriter returnOutput, StringWriter paramsOutput)
        {
            return CreateBareClassProxy(returnOutput, paramsOutput, new HashSet<string>());
        }

        private static ITestDiagnosticLoggingService CreateBareClassProxy(
            StringWriter returnOutput,
            StringWriter paramsOutput,
            HashSet<string> filterTheseParameters)
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .WriteTo.TextWriter(returnOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ReturnValueName}}}")
                .WriteTo.TextWriter(paramsOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ArgumentsName}}}")
                .CreateLogger();

            var instance = new TestBareDiagnosticLoggingService();
            var interceptor = new AsyncDiagnosticLoggingInterceptor(serilog, filterTheseParameters);

            ITestDiagnosticLoggingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestDiagnosticLoggingService>(instance, interceptor.ToInterceptor());
            return proxy;
        }

        #region Plain

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassNoParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.NoParamsReturnVoid();

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassSomeParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassNoParamsReturnString_OutputEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.NoParamsReturnString();

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassSomeParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        #endregion

        #region Active

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveNoParamsReturnVoid_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveNoParamsReturnVoid();

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnVoid_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveNoParamsReturnString_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveNoParamsReturnString();

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnString_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnStringFilterTheseParameters_ParameterOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnStringDoNotFilterTheseParameters_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"random1", @"random2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassActiveSomeParamsReturnStringActiveParamsActiveReturnFilterTheseParameters_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        #endregion

        #region Inactive

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveNoParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveNoParamsReturnVoid();

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveSomeParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveNoParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveNoParamsReturnString();

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveSomeParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_BareClassInactiveSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestBareDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        #endregion
    }
}
