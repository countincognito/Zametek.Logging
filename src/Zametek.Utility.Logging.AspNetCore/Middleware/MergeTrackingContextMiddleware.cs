using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging
{
    public class MergeTrackingContextMiddleware
    {
        #region Fields

        private readonly RequestDelegate m_Next;

        #endregion

        #region Ctors

        public MergeTrackingContextMiddleware(RequestDelegate next)
        {
            m_Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        #endregion

        #region Public Members

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            MergeTrackingContextHeader(httpContext);

            // In order to add http headers to the message response, reprocess the Tracking Context
            // in a callback function. This is necessary because response headers cannot be set after
            // the response body has been written.
            httpContext.Response.OnStarting(state =>
            {
                var ctx = (HttpContext)state;

                // Take the current Tracking Context (if it exists) and add it to the http response headers.
                TrackingContextHelper.ProcessHttpHeaders(ctx?.Response?.Headers);
                return Task.FromResult(0);
            }, httpContext);

            await m_Next.Invoke(httpContext).ConfigureAwait(false);
        }

        #endregion

        #region Private Members

        private static void MergeTrackingContextHeader(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            TrackingContext previousTrackingContext = TrackingContext.Current;

            // Take the Tracking Context (if it exists) from the http request headers and make it current.
            TrackingContextHelper.ProcessHttpHeaders(httpContext.Request?.Headers);

            // Check to see how the new current Tracking Context compares to the previous one.
            if (previousTrackingContext != null)
            {
                TrackingContext currentTrackingContext = TrackingContext.Current;

                Debug.Assert(currentTrackingContext != null);

                // Prepare the inputs for the new Tracking Context.
                Guid callChainId = currentTrackingContext.CallChainId;
                DateTime originatorUtcTimestamp = currentTrackingContext.OriginatorUtcTimestamp;
                IDictionary<string, string> extraHeaders = currentTrackingContext
                    .ExtraHeaders
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Extract the extra headers that were in the previous Tracking Context, but
                // are not in the inputs for the new Tracking Context.
                IList<string> nowMissingHeaderNames = previousTrackingContext
                    .ExtraHeaders.Keys
                    .Except(extraHeaders.Keys)
                    .ToList();

                // Add the missing extra headers back to the inputs for the new Tracking Context.
                foreach (string headerName in nowMissingHeaderNames)
                {
                    extraHeaders.Add(headerName, previousTrackingContext.ExtraHeaders[headerName]);
                }

                // Extract the extra headers that were in the previous Tracking Context, and
                // also in inputs for the new Tracking Context, but whose values are not empty.
                IList<string> sharedHeaderNames = extraHeaders
                    .Keys
                    .Except(nowMissingHeaderNames)
                    .ToList();

                // Add the missing extra header values back to the inputs for the new Tracking Context.
                foreach (string headerName in sharedHeaderNames)
                {
                    if (string.IsNullOrWhiteSpace(extraHeaders[headerName]))
                    {
                        extraHeaders[headerName] = previousTrackingContext.ExtraHeaders[headerName];
                    }
                }

                // Now recreate the current Tracking Context with the refilled extra headers.
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
