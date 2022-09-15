using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Zametek.Utility.Logging
{
    public static class TrackingContextHelper
    {
        #region Fields

        private const string c_InvalidHeaderKeyCharacters = @"[^A-Z0-9_-]";
        public static readonly string TrackingContextKeyName =
            Regex.Replace(TrackingContext.FullName.ToUpperInvariant(), c_InvalidHeaderKeyCharacters, "-");

        #endregion

        #region Public Members

        /// <summary>
        /// Extract Tracking Context from http headers, or create a new Tracking Context and
        /// add it back to the http headers.
        /// </summary>
        public static IHeaderDictionary ProcessHttpHeaders(IHeaderDictionary headers)
        {
            if (headers is null)
            {
                return headers;
            }

            // Retrieve the tracking context from the message header, if it exists.
            if (headers.TryGetValue(TrackingContextKeyName, out StringValues values))
            {
                CreateTrackingContextFromHeaderValues(values);
            }
            else
            {
                // Create a new Tracking Context and copy it to the message header.
                headers.Add(TrackingContextKeyName, CreateAndSerializeNewTrackingContext());
            }

            return headers;
        }

        /// <summary>
        /// Extract Tracking Context from http headers, or create a new Tracking Context and
        /// add it back to the http headers.
        /// </summary>
        public static HttpHeaders ProcessHttpHeaders(HttpHeaders headers)
        {
            if (headers is null)
            {
                return headers;
            }

            // Retrieve the tracking context from the message header, if it exists.
            if (headers.TryGetValues(TrackingContextKeyName, out IEnumerable<string> values))
            {
                CreateTrackingContextFromHeaderValues(values);
            }
            else
            {
                // Create a new Tracking Context and copy it to the message header.
                headers.Add(TrackingContextKeyName, CreateAndSerializeNewTrackingContext());
            }

            return headers;
        }

        #endregion

        #region Private Members

        private static void CreateTrackingContextFromHeaderValues(IEnumerable<string> values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (values.Count() > 1)
            {
                throw new InvalidOperationException(Properties.Resources.CannotHaveMoreThanOneSerializedTrackingContextInHTTPHeaders);
            }

            string tcBase64 = values.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(tcBase64))
            {
                TrackingContext.NewCurrentIfEmpty();
            }
            else
            {
                // If an tracking context exists in the message header then use it to replace the current context.
                TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());
                tc.SetAsCurrent();
            }
        }

        private static string CreateAndSerializeNewTrackingContext()
        {
            TrackingContext.NewCurrentIfEmpty();
            Debug.Assert(TrackingContext.Current != null);
            byte[] byteArray = TrackingContext.Serialize(TrackingContext.Current);
            return byteArray.ByteArrayToBase64String();
        }

        #endregion
    }
}
