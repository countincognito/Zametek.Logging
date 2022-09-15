using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging
{
    public class AsyncTrackingInterceptor
        : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceedInfo is null)
            {
                throw new ArgumentNullException(nameof(proceedInfo));
            }
            if (proceed is null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            TrackingContext.NewCurrentIfEmpty();
            await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }

        protected override async Task<T> InterceptAsync<T>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<T>> proceed)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceedInfo is null)
            {
                throw new ArgumentNullException(nameof(proceedInfo));
            }
            if (proceed is null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            TrackingContext.NewCurrentIfEmpty();
            return await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }
    }
}
