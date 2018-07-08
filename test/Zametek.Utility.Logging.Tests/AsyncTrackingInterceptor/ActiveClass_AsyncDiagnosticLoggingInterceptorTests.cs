using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace Zametek.Utility.Logging.Tests
{
    public partial class AsyncDiagnosticLoggingInterceptorTests
    {
        private static ITestDiagnosticLoggingService CreateActiveClassProxy(StringWriter returnOutput, StringWriter paramsOutput)
        {
            return CreateActiveClassProxy(returnOutput, paramsOutput, new HashSet<string>());
        }

        private static ITestDiagnosticLoggingService CreateActiveClassProxy(
            StringWriter returnOutput,
            StringWriter paramsOutput,
            HashSet<string> filterTheseParameters)
        {
            ILogger serilog = new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .WriteTo.TextWriter(returnOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ReturnValueName}}}")
                .WriteTo.TextWriter(paramsOutput, outputTemplate: $"{{{AsyncDiagnosticLoggingInterceptor.ArgumentsName}}}")
                .CreateLogger();

            var instance = new TestActiveDiagnosticLoggingService();
            var interceptor = new AsyncDiagnosticLoggingInterceptor(serilog, filterTheseParameters);

            ITestDiagnosticLoggingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestDiagnosticLoggingService>(instance, interceptor.ToInterceptor());
            return proxy;
        }

        #region Plain

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassNoParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.NoParamsReturnVoid();

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassSomeParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassNoParamsReturnString_OutputEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.NoParamsReturnString();

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassSomeParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        #endregion

        #region Active

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveNoParamsReturnVoid_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveNoParamsReturnVoid();

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnVoid_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveNoParamsReturnString_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveNoParamsReturnString();

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_NoParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnString_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnStringFilterTheseParameters_ParameterOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_FilteredParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnStringDoNotFilterTheseParameters_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"random1", @"random2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassActiveSomeParamsReturnStringActiveParamsActiveReturnFilterTheseParameters_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        #endregion

        #region Inactive

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveNoParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveNoParamsReturnVoid();

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveSomeParamsReturnVoid_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoid(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveNoParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveNoParamsReturnString();

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveSomeParamsReturnString_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnString(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveSomeParamsReturnVoidActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(AsyncDiagnosticLoggingInterceptor.VoidSubstitute, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveSomeParamsReturnStringActiveParamsActiveReturn_OutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringActiveParamsActiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(returnValue, returnOutput.ToString());
            Assert.AreEqual(m_ParamsLogReturn, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveSomeParamsReturnVoidInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        [TestMethod]
        public void AsyncDiagnosticLoggingInterceptor_ActiveClassInactiveSomeParamsReturnStringInactiveParamsInactiveReturn_OutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateActiveClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringInactiveParamsInactiveReturn(m_FirstParam, m_SecondParam);

            Assert.AreEqual(TestActiveDiagnosticLoggingService.ReturnValue, returnValue);
            Assert.AreEqual(string.Empty, returnOutput.ToString());
            Assert.AreEqual(string.Empty, paramsOutput.ToString());
        }

        #endregion
    }
}
