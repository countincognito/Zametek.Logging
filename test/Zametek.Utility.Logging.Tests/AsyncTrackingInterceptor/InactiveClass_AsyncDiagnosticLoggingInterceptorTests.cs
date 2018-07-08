using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace Zametek.Utility.Logging.Tests
{
    public partial class AsyncDiagnosticLoggingInterceptorTests
    {
        private static ITestDiagnosticLoggingService CreateInactiveClassProxy(StringWriter returnOutput, StringWriter paramsOutput)
        {
            return CreateInactiveClassProxy(returnOutput, paramsOutput, new HashSet<string>());
        }

        private static ITestDiagnosticLoggingService CreateInactiveClassProxy(
            StringWriter returnOutput,
            StringWriter paramsOutput,
            HashSet<string> filterTheseParameters)
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .WriteTo.TextWriter(returnOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ReturnValueName}}}")
                .WriteTo.TextWriter(paramsOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ArgumentsName}}}")
                .CreateLogger();

            var instance = new TestInactiveDiagnosticLoggingService();
            var interceptor = new AsyncDiagnosticLoggingInterceptor(serilog, filterTheseParameters);

            ITestDiagnosticLoggingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestDiagnosticLoggingService>(instance, interceptor.ToInterceptor());
            return proxy;
        }

        #region Plain

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassNoParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.NoParamsReturnVoid();

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassSomeParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassNoParamsReturnString_OutputEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.NoParamsReturnString();

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassSomeParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        #endregion

        #region Active

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveNoParamsReturnVoid_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveNoParamsReturnVoid();

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnVoid_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveNoParamsReturnString_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveNoParamsReturnString();

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnString_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnStringFilterTheseParameters_ParameterOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnStringDoNotFilterTheseParameters_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"random1", @"random2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassActiveSomeParamsReturnStringActiveParamsActiveReturnFilterTheseParameters_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        #endregion

        #region Inactive

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveNoParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveNoParamsReturnVoid();

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveSomeParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveNoParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveNoParamsReturnString();

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveSomeParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_InactiveClassInactiveSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateInactiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestInactiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        #endregion
    }
}
