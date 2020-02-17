using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Zametek.Utility.Logging.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTrackingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TrackingMiddleware>();
        }

        public static IApplicationBuilder UseTrackingMiddleware(
            this IApplicationBuilder builder,
            Func<IDictionary<string, string>> setupFunc)
        {
            return builder.UseMiddleware<TrackingMiddleware>(setupFunc);
        }

        public static IApplicationBuilder UseTrackingMiddleware(
            this IApplicationBuilder builder,
            Func<HttpContext, IDictionary<string, string>> setupFunc)
        {
            return builder.UseMiddleware<TrackingMiddleware>(setupFunc);
        }
    }
}
