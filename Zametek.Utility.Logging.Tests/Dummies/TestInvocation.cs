using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Zametek.Utility.Logging.Tests
{
    public partial class InvocationEnricherTests
    {
        public class TestInvocation
            : IInvocation
        {
            public object[] Arguments => throw new NotImplementedException();

            public Type[] GenericArguments => throw new NotImplementedException();

            public object InvocationTarget => throw new NotImplementedException();

            public MethodInfo Method => typeof(Test).GetMethod(nameof(Test.ReturnAsync));

            public MethodInfo MethodInvocationTarget => throw new NotImplementedException();

            public object Proxy => throw new NotImplementedException();

            public object ReturnValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public Type TargetType => typeof(Test);

            public object GetArgumentValue(int index)
            {
                throw new NotImplementedException();
            }

            public MethodInfo GetConcreteMethod()
            {
                throw new NotImplementedException();
            }

            public MethodInfo GetConcreteMethodInvocationTarget()
            {
                throw new NotImplementedException();
            }

            public void Proceed()
            {
                throw new NotImplementedException();
            }

            public void SetArgumentValue(int index, object value)
            {
                throw new NotImplementedException();
            }
        }
    }
}
