using System;

namespace Zametek.Utility.Logging.Tests
{
    [DiagnosticLogging(LogActive.On)]
    public class TestActiveDiagnosticLoggingService
        : ITestDiagnosticLoggingService
    {
        public string ActiveNoParamsReturnString()
        {
            throw new NotImplementedException();
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

        public string NoParamsReturnString()
        {
            throw new NotImplementedException();
        }

        public void NoParamsReturnVoid()
        {
            throw new NotImplementedException();
        }

        public string SomeParamsReturnString(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public string SomeParamsReturnStringActiveParamsActiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public string SomeParamsReturnStringInactiveParamsInactiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void SomeParamsReturnVoid(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void SomeParamsReturnVoidActiveParamsActiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }

        public void SomeParamsReturnVoidInactiveParamsInactiveReturn(string param1, string param2)
        {
            throw new NotImplementedException();
        }
    }
}
