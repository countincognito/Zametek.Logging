using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Zametek.Utility.Logging.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        #region TrackingContextMiddleware

        public static IApplicationBuilder UseTrackingContextMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TrackingContextMiddleware>();
        }

        public static IApplicationBuilder UseTrackingContextMiddleware(
            this IApplicationBuilder builder,
            Func<IDictionary<string, string>> setupFunc)
        {
            return builder.UseMiddleware<TrackingContextMiddleware>(setupFunc);
        }

        public static IApplicationBuilder UseTrackingContextMiddleware(
            this IApplicationBuilder builder,
            Func<HttpContext, IDictionary<string, string>> setupFunc)
        {
            return builder.UseMiddleware<TrackingContextMiddleware>(setupFunc);
        }

        #endregion

        #region MergeTrackingContextMiddleware

        public static IApplicationBuilder UseMergeTrackingContextMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MergeTrackingContextMiddleware>();
        }

        #endregion

        #region OverwriteTrackingContextHeaderMiddleware

        public static IApplicationBuilder UseOverwriteTrackingContextHeaderMiddleware(
            this IApplicationBuilder builder,
            HashSet<string> headerKeys,
            Func<HttpContext, string> headerValueGenerator)
        {
            return builder.UseMiddleware<OverwriteTrackingContextHeaderMiddleware>(
                headerKeys,
                headerValueGenerator);
        }

        public static IApplicationBuilder UseOverwriteTrackingContextHeaderMiddleware(
            this IApplicationBuilder builder,
            string headerKey,
            Func<HttpContext, string> headerValueGenerator)
        {
            return builder.UseOverwriteTrackingContextHeaderMiddleware(
                new HashSet<string>(new[] { headerKey }),
                headerValueGenerator);
        }

        public static IApplicationBuilder UseOverwriteTrackingContextHeaderMiddleware(
            this IApplicationBuilder builder,
            string headerKey,
            string headerValue)
        {
            return builder.UseOverwriteTrackingContextHeaderMiddleware(
                headerKey,
                _ => headerValue);
        }

        public static IApplicationBuilder UseOverwriteTrackingContextHeaderMiddleware(
            this IApplicationBuilder builder,
            string headerKey)
        {
            return builder.UseOverwriteTrackingContextHeaderMiddleware(
                headerKey,
                string.Empty);
        }

        #endregion
    }
}
