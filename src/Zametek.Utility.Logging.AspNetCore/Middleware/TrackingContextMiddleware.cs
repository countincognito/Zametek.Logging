using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.AspNetCore
{
    public class TrackingContextMiddleware
    {
        #region Fields

        private readonly RequestDelegate m_Next;
        private readonly Func<HttpContext, IDictionary<string, string>> m_SetupFunc;

        #endregion

        #region Ctors

        public TrackingContextMiddleware(RequestDelegate next)
            : this(next, _ => new Dictionary<string, string>())
        {
        }

        public TrackingContextMiddleware(
            RequestDelegate next,
            Func<IDictionary<string, string>> setupFunc)
        {
            if (setupFunc == null)
            {
                throw new ArgumentNullException(nameof(setupFunc));
            }
            m_Next = next ?? throw new ArgumentNullException(nameof(next));
            m_SetupFunc = _ => setupFunc.Invoke();
        }

        public TrackingContextMiddleware(
            RequestDelegate next,
            Func<HttpContext, IDictionary<string, string>> setupFunc)
        {
            m_Next = next ?? throw new ArgumentNullException(nameof(next));
            m_SetupFunc = setupFunc ?? throw new ArgumentNullException(nameof(setupFunc));
        }

        #endregion

        #region Public Members

        public async Task Invoke(HttpContext httpContext)
        {
            IDictionary<string, string> extraHeaders = m_SetupFunc?.Invoke(httpContext) ?? new Dictionary<string, string>();
            Debug.Assert(extraHeaders != null);
            TrackingContext.NewCurrentIfEmpty(extraHeaders);

            using (LogContext.Push(new TrackingContextEnricher()))
            {
                // Must await the next middleware.
                // If not the log context will unwind at the first asyncronious operation.
                await m_Next.Invoke(httpContext).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
