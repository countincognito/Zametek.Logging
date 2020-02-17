namespace Zametek.Utility.Logging.Tests
{
    public class TestBareDiagnosticLoggingService
        : ITestDiagnosticLoggingService
    {
        public const string ReturnValue = @"MyBareReturnString";

        #region Plain

        public void NoParamsReturnVoid()
        {
        }

        public void SomeParamsReturnVoid(string param1, string param2)
        {
        }

        public string NoParamsReturnString()
        {
            return ReturnValue;
        }

        public string SomeParamsReturnString(string param1, string param2)
        {
            return ReturnValue;
        }

        [return: DiagnosticLogging(LogActive.On)]
        public void SomeParamsReturnVoidActiveParamsActiveReturn([DiagnosticLogging(LogActive.On)] string param1, [DiagnosticLogging(LogActive.On)] string param2)
        {
        }

        [return: DiagnosticLogging(LogActive.On)]
        public string SomeParamsReturnStringActiveParamsActiveReturn([DiagnosticLogging(LogActive.On)] string param1, [DiagnosticLogging(LogActive.On)] string param2)
        {
            return ReturnValue;
        }

        [return: DiagnosticLogging(LogActive.Off)]
        public void SomeParamsReturnVoidInactiveParamsInactiveReturn([DiagnosticLogging(LogActive.Off)] string param1, [DiagnosticLogging(LogActive.Off)] string param2)
        {
        }

        [return: DiagnosticLogging(LogActive.Off)]
        public string SomeParamsReturnStringInactiveParamsInactiveReturn([DiagnosticLogging(LogActive.Off)] string param1, [DiagnosticLogging(LogActive.Off)] string param2)
        {
            return ReturnValue;
        }

        #endregion

        #region Active

        [DiagnosticLogging(LogActive.On)]
        public void ActiveNoParamsReturnVoid()
        {
        }

        [DiagnosticLogging(LogActive.On)]
        public void ActiveSomeParamsReturnVoid(string param1, string param2)
        {
        }

        [DiagnosticLogging(LogActive.On)]
        public string ActiveNoParamsReturnString()
        {
            return ReturnValue;
        }

        [DiagnosticLogging(LogActive.On)]
        public string ActiveSomeParamsReturnString(string param1, string param2)
        {
            return ReturnValue;
        }

        [DiagnosticLogging(LogActive.On)]
        [return: DiagnosticLogging(LogActive.On)]
        public void ActiveSomeParamsReturnVoidActiveParamsActiveReturn([DiagnosticLogging(LogActive.On)] string param1, [DiagnosticLogging(LogActive.On)] string param2)
        {
        }

        [DiagnosticLogging(LogActive.On)]
        [return: DiagnosticLogging(LogActive.On)]
        public string ActiveSomeParamsReturnStringActiveParamsActiveReturn([DiagnosticLogging(LogActive.On)] string param1, [DiagnosticLogging(LogActive.On)] string param2)
        {
            return ReturnValue;
        }

        [DiagnosticLogging(LogActive.On)]
        [return: DiagnosticLogging(LogActive.Off)]
        public void ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn([DiagnosticLogging(LogActive.Off)] string param1, [DiagnosticLogging(LogActive.Off)] string param2)
        {
        }

        [DiagnosticLogging(LogActive.On)]
        [return: DiagnosticLogging(LogActive.Off)]
        public string ActiveSomeParamsReturnStringInactiveParamsInactiveReturn([DiagnosticLogging(LogActive.Off)] string param1, [DiagnosticLogging(LogActive.Off)] string param2)
        {
            return ReturnValue;
        }

        #endregion

        #region Inactive

        [DiagnosticLogging(LogActive.Off)]
        public void InactiveNoParamsReturnVoid()
        {
        }

        [DiagnosticLogging(LogActive.Off)]
        public void InactiveSomeParamsReturnVoid(string param1, string param2)
        {
        }

        [DiagnosticLogging(LogActive.Off)]
        public string InactiveNoParamsReturnString()
        {
            return ReturnValue;
        }

        [DiagnosticLogging(LogActive.Off)]
        public string InactiveSomeParamsReturnString(string param1, string param2)
        {
            return ReturnValue;
        }

        [DiagnosticLogging(LogActive.Off)]
        [return: DiagnosticLogging(LogActive.On)]
        public void InactiveSomeParamsReturnVoidActiveParamsActiveReturn([DiagnosticLogging(LogActive.On)] string param1, [DiagnosticLogging(LogActive.On)] string param2)
        {
        }

        [DiagnosticLogging(LogActive.Off)]
        [return: DiagnosticLogging(LogActive.On)]
        public string InactiveSomeParamsReturnStringActiveParamsActiveReturn([DiagnosticLogging(LogActive.On)] string param1, [DiagnosticLogging(LogActive.On)] string param2)
        {
            return ReturnValue;
        }

        [DiagnosticLogging(LogActive.Off)]
        [return: DiagnosticLogging(LogActive.Off)]
        public void InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn([DiagnosticLogging(LogActive.Off)] string param1, [DiagnosticLogging(LogActive.Off)] string param2)
        {
        }

        [DiagnosticLogging(LogActive.Off)]
        [return: DiagnosticLogging(LogActive.Off)]
        public string InactiveSomeParamsReturnStringInactiveParamsInactiveReturn([DiagnosticLogging(LogActive.Off)] string param1, [DiagnosticLogging(LogActive.Off)] string param2)
        {
            return ReturnValue;
        }

        #endregion
    }
}
