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
        private static ITestDiagnosticLoggingService CreateGivenInactiveClass_WhenProxy(StringWriter returnOutput, StringWriter paramsOutput)
        {
            return CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput, new HashSet<string>());
        }

        private static ITestDiagnosticLoggingService CreateGivenInactiveClass_WhenProxy(
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

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenNoParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.NoParamsReturnVoid();

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenSomeParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoid(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenNoParamsReturnString_ThenOutputEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.NoParamsReturnString();

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenSomeParamsReturnString_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenSomeParamsReturnVoidActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenSomeParamsReturnStringActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenSomeParamsReturnVoidInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.SomeParamsReturnVoidInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenSomeParamsReturnStringInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.SomeParamsReturnStringInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        #endregion

        #region Active

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveNoParamsReturnVoid_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.ActiveNoParamsReturnVoid();

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_NoParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnVoid_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoid(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveNoParamsReturnString_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveNoParamsReturnString();

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_NoParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnString_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnVoidActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnStringActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnVoidInactiveParamsInactiveReturn_ThenOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute);
            paramsOutput.ToString().ShouldBe(s_FilteredParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnStringInactiveParamsInactiveReturn_ThenOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.ActiveSomeParamsReturnStringInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.FilteredParameterSubstitute);
            paramsOutput.ToString().ShouldBe(s_FilteredParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnStringFilterTheseParameters_ThenParameterOutputsFiltered()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_FilteredParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnStringDoNotFilterTheseParameters_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput, new HashSet<string> { @"random1", @"random2" });

            string returnValue = proxy.ActiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenActiveSomeParamsReturnStringActiveParamsActiveReturnFilterTheseParameters_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput, new HashSet<string> { @"param1", @"param2" });

            string returnValue = proxy.ActiveSomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        #endregion

        #region Inactive

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveNoParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.InactiveNoParamsReturnVoid();

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveSomeParamsReturnVoid_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoid(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveNoParamsReturnString_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveNoParamsReturnString();

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveSomeParamsReturnString_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnString(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveSomeParamsReturnVoidActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBe(AsyncDiagnosticLoggingInterceptor.VoidSubstitute);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveSomeParamsReturnStringActiveParamsActiveReturn_ThenOutputsPopulated()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringActiveParamsActiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBe(returnValue);
            paramsOutput.ToString().ShouldBe(s_ParamsLogReturn);
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveSomeParamsReturnVoidInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            proxy.InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        [Fact]
        public void AsyncDiagnosticLoggingInterceptor_GivenInactiveClass_WhenInactiveSomeParamsReturnStringInactiveParamsInactiveReturn_ThenOutputsEmpty()
        {
            var returnOutput = new StringWriter();
            var paramsOutput = new StringWriter();

            ITestDiagnosticLoggingService proxy = CreateGivenInactiveClass_WhenProxy(returnOutput, paramsOutput);

            string returnValue = proxy.InactiveSomeParamsReturnStringInactiveParamsInactiveReturn(s_FirstParam, s_SecondParam);

            returnValue.ShouldBe(TestInactiveDiagnosticLoggingService.ReturnValue);
            returnOutput.ToString().ShouldBeEmpty();
            paramsOutput.ToString().ShouldBeEmpty();
        }

        #endregion
    }
}
