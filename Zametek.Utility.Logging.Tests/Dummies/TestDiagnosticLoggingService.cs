using System;

namespace Zametek.Utility.Logging.Tests
{
    public class TestDiagnosticLoggingService
        : ITestDiagnosticLoggingService
    {
        public const string ReturnValue = "MyReturnString";

        

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

























        public string ActiveNoParamsReturnString()
        {
            return ReturnValue;
        }

        public void ActiveNoParamsReturnVoid()
        {
            throw new NotImplementedException();
        }

        public string ActiveSomeParamsReturnString(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public string ActiveSomeParamsReturnStringActiveParamsActiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public string ActiveSomeParamsReturnStringInactiveParamsInactiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void ActiveSomeParamsReturnVoid(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void ActiveSomeParamsReturnVoidActiveParamsActiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public string InactiveNoParamsReturnString()
        {
            throw new NotImplementedException();
        }

        public void InactiveNoParamsReturnVoid()
        {
            throw new NotImplementedException();
        }

        public string InactiveSomeParamsReturnString(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public string InactiveSomeParamsReturnStringActiveParamsActiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public string InactiveSomeParamsReturnStringInactiveParamsInactiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void InactiveSomeParamsReturnVoid(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void InactiveSomeParamsReturnVoidActiveParamsActiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }




    }
}
