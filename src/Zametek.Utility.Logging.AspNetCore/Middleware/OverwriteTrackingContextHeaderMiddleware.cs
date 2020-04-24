using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging
{
    /// <summary>
    /// Use this to overwrite any extra header values are in the current Tracking Context.
    /// For example: if a user ID has come in from an external Tracking Context, then you
    /// almost certainly want to overwrite it with an authenticated user ID.
    /// </summary>
    public class OverwriteTrackingContextHeaderMiddleware
    {
        #region Fields

        private readonly RequestDelegate m_Next;
        private readonly IList<string> m_HeaderKeys;
        private readonly Func<HttpContext, string> m_HeaderValueGenerator;

        #endregion

        #region Ctors

        public OverwriteTrackingContextHeaderMiddleware(
            RequestDelegate next,
            HashSet<string> headerKeys,
            Func<HttpContext, string> headerValueGenerator)
        {
            m_Next = next ?? throw new ArgumentNullException(nameof(next));
            m_HeaderKeys = headerKeys.ToList();
            m_HeaderValueGenerator = headerValueGenerator ?? throw new ArgumentNullException(nameof(headerValueGenerator));
        }

        #endregion

        #region Public Members

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            OverwriteTrackingContextHeader(httpContext, m_HeaderKeys, m_HeaderValueGenerator);

            // In order to add http headers to the message response, reprocess the Tracking Context
            // in a callback function. This is necessary because response headers cannot be set after
            // the response body has been written.
            httpContext.Response.OnStarting(state =>
            {
                var ctx = (HttpContext)state;

                OverwriteTrackingContextHeader(httpContext, m_HeaderKeys, m_HeaderValueGenerator);

                // Take the current Tracking Context (if it exists) and add it to the http response headers.
                TrackingContextHelper.ProcessHttpHeaders(ctx?.Response?.Headers);
                return Task.FromResult(0);
            }, httpContext);

            await m_Next.Invoke(httpContext).ConfigureAwait(false);
        }

        #endregion

        #region Private Members

        private static void OverwriteTrackingContextHeader(
            HttpContext httpContext,
            IList<string> headerKeys,
            Func<HttpContext, string> headerValueGenerator)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            if (headerKeys == null)
            {
                throw new ArgumentNullException(nameof(headerKeys));
            }
            if (headerValueGenerator == null)
            {
                throw new ArgumentNullException(nameof(headerValueGenerator));
            }

            TrackingContext currentTrackingContext = TrackingContext.Current;

            // Replace the http header value in the current Tracking Context, if the
            // http header exists.
            if (currentTrackingContext != null)
            {
                // Prepare the inputs for the new Tracking Context.
                Guid callChainId = currentTrackingContext.CallChainId;
                DateTime originatorUtcTimestamp = currentTrackingContext.OriginatorUtcTimestamp;
                IDictionary<string, string> extraHeaders = currentTrackingContext
                    .ExtraHeaders
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Cycle through each of the requested http header keys.
                foreach (string headerKey in headerKeys)
                {
                    if (extraHeaders.ContainsKey(headerKey))
                    {
                        // Overwrite the target http header value.
                        string headerValue = headerValueGenerator(httpContext);
                        extraHeaders[headerKey] = headerValue;
                    }
                }

                // Now recreate the current Tracking Context with the overwritten extra headers.
                var newTrackingContext = new TrackingContext(
                    callChainId,
                    originatorUtcTimestamp,
                    extraHeaders);
                newTrackingContext.SetAsCurrent();
            }
        }

        #endregion
    }
}
