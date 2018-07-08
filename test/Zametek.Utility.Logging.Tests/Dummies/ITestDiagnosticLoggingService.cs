namespace Zametek.Utility.Logging.Tests
{
    public interface ITestDiagnosticLoggingService
    {
        void NoParamsReturnVoid();

        void SomeParamsReturnVoid(string param1, string param2);

        string NoParamsReturnString();

        string SomeParamsReturnString(string param1, string param2);

        void SomeParamsReturnVoidActiveParamsActiveReturn(string param1, string param2);

        string SomeParamsReturnStringActiveParamsActiveReturn(string param1, string param2);

        void SomeParamsReturnVoidInactiveParamsInactiveReturn(string param1, string param2);

        string SomeParamsReturnStringInactiveParamsInactiveReturn(string param1, string param2);



        void ActiveNoParamsReturnVoid();

        void ActiveSomeParamsReturnVoid(string param1, string param2);

        string ActiveNoParamsReturnString();

        string ActiveSomeParamsReturnString(string param1, string param2);

        void ActiveSomeParamsReturnVoidActiveParamsActiveReturn(string param1, string param2);

        string ActiveSomeParamsReturnStringActiveParamsActiveReturn(string param1, string param2);

        void ActiveSomeParamsReturnVoidInactiveParamsInactiveReturn(string param1, string param2);

        string ActiveSomeParamsReturnStringInactiveParamsInactiveReturn(string param1, string param2);



        void InactiveNoParamsReturnVoid();

        void InactiveSomeParamsReturnVoid(string param1, string param2);

        string InactiveNoParamsReturnString();

        string InactiveSomeParamsReturnString(string param1, string param2);

        void InactiveSomeParamsReturnVoidActiveParamsActiveReturn(string param1, string param2);

        string InactiveSomeParamsReturnStringActiveParamsActiveReturn(string param1, string param2);

        void InactiveSomeParamsReturnVoidInactiveParamsInactiveReturn(string param1, string param2);

        string InactiveSomeParamsReturnStringInactiveParamsInactiveReturn(string param1, string param2);
    }
}
