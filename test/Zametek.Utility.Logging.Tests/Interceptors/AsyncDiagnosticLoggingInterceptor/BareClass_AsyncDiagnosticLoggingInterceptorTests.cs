using Castle.DynamicProxy;
using Serilog;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using Xunit;

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
                .WriteTo.TextWriter(returnOutput, outputTemplate: $@"{{{AsyncDiagnosticLoggingInterceptor.ReturnValueName}}}")
                .WriteTo.TextWriter(paramsOutput, outputTemplate: $@"{{{AsyncDiagnosticLoggingInterceptor.ArgumentsName}}}")
                .CreateLogger();

            var instance = new TestBareDiagnosticLoggingService();
            var interceptor = new AsyncDiagnosticLoggingInterceptor(serilog, filterTheseParameters);

            ITestDiagnosticLoggingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestDiagnosticLoggingService>(instance, interceptor.ToInterceptor());
            return proxy;
        }

        #region Plain

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenNoParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.NoParamsReturnVoid();

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenSomeParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoid(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenNoParamsReturnString_ThenOutputEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.NoParamsReturnString();

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenSomeParamsReturnString_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenSomeParamsReturnVoidActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenSomeParamsReturnStringActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenSomeParamsReturnVoidInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenSomeParamsReturnStringInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        #endregion

        #region Active

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveNoParamsReturnVoid_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveNoParamsReturnVoid();

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_NoParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnVoid_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoid(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveNoParamsReturnString_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveNoParamsReturnString();

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_NoParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnString_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnVoidActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnStringActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnVoidInactiveParamsInactiveReturn_ThenOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute);
            paramsOutput.ToString().ShouldBe(s_FilteredParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnStringInactiveParamsInactiveReturn_ThenOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute);
            paramsOutput.ToString().ShouldBe(s_FilteredParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnStringFilterTheseParameters_ThenParameterOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_FilteredParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnStringDoNotFilterTheseParameters_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"random1", @"random2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenActiveSomeParamsReturnStringActiveParamsActiveReturnFilterTheseParameters_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        #endregion

        #region Inactive

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveNoParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveNoParamsReturnVoid();

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveSomeParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoid(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveNoParamsReturnString_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveNoParamsReturnString();

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveSomeParamsReturnString_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveSomeParamsReturnVoidActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveSomeParamsReturnStringActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveSomeParamsReturnVoidInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenBareClass_WhenInactiveSomeParamsReturnStringInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateBareClassProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestBareDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        #endregion
    }
}
