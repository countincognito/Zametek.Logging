using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Logging.AspNetCore.Tests
{
    public class MiddlewareTests
    {
        [Fact]
        public async Task Startup_GivenTrackingContextMiddleware_WhenNoTrackingContextSent_ThenNoTrackingContextReturned()
        {
            var textWriterSink = new StringWriter();
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                    webHost.ConfigureServices((services) => StartupBase.ConfigureServicesWithLogSink(
                        services,
                        textWriterSink,
                        $"{{{nameof(TrackingContext.CallChainId)}}}{textWriterSink.NewLine}"));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            TrackingContext.Current.ShouldBeNull();

            var response = await client.GetAsync(@"/api/values");
            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).ShouldBeFalse();

            TrackingContext.Current.ShouldBeNull();

            List<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count.ShouldBe(10);

            //callChainIds.All(x => x.Equals(tc.CallChainId.ToDashedString())).ShouldBeTrue();
        }

        [Fact]
        public async Task Startup_GivenTrackingContextMiddleware_WhenTrackingContextSent_ThenNoTrackingContextReturned()
        {
            var textWriterSink = new StringWriter();
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                    webHost.ConfigureServices((services) => StartupBase.ConfigureServicesWithLogSink(
                        services,
                        textWriterSink,
                        $"{{{nameof(TrackingContext.CallChainId)}}}{textWriterSink.NewLine}"));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            TrackingContext.NewCurrentIfEmpty();
            TrackingContext currentContext = TrackingContext.Current;
            currentContext.ShouldNotBeNull();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, @"/api/values");

            string tcHeader = TrackingContext.Serialize(currentContext).ByteArrayToBase64String();
            requestMessage.Headers.Add(TrackingContextHelper.TrackingContextKeyName, tcHeader);

            var response = await client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).ShouldBeFalse();

            TrackingContext.Current.ShouldNotBeNull();
            TrackingContext.Current.ShouldBeEquivalentTo(currentContext);

            List<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count.ShouldBe(10);

            // The call chain ID should be different on the server side.
            callChainIds.All(x => !x.Equals(currentContext.CallChainId.ToDashedString())).ShouldBeTrue();
        }

        [Fact]
        public async Task Startup_GivenTrackingContextWithMergeMiddleware_WhenNoTrackingContextSent_ThenTrackingContextReturned()
        {
            var textWriterSink = new StringWriter();
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<StartupWithMerge>();
                    webHost.ConfigureServices((services) => StartupBase.ConfigureServicesWithLogSink(
                        services,
                        textWriterSink,
                        $"{{{nameof(TrackingContext.CallChainId)}}}{textWriterSink.NewLine}"));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            TrackingContext.Current.ShouldBeNull();

            var response = await client.GetAsync(@"/api/values");
            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).ShouldBeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.ShouldNotBe(Guid.Empty);
            tc.OriginatorUtcTimestamp.ShouldBe(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.ShouldBe(3);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].ShouldNotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].ShouldBe(@"UK");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].ShouldNotBeNullOrWhiteSpace();

            TrackingContext.Current.ShouldBeNull();

            List<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count.ShouldBe(10);

            callChainIds.All(x => x.Equals(tc.CallChainId.ToDashedString())).ShouldBeTrue();
        }

        [Fact]
        public async Task Startup_GivenTrackingContextWithMergeMiddleware_WhenTrackingContextSent_ThenTrackingContextReturned()
        {
            var textWriterSink = new StringWriter();
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<StartupWithMerge>();
                    webHost.ConfigureServices((services) => StartupBase.ConfigureServicesWithLogSink(
                        services,
                        textWriterSink,
                        $"{{{nameof(TrackingContext.CallChainId)}}}{textWriterSink.NewLine}"));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            string headerKey = Guid.NewGuid().ToFlatString();
            string headerValue = Guid.NewGuid().ToFlatString();
            TrackingContext.NewCurrentIfEmpty(new Dictionary<string, string>()
            {
                { headerKey, headerValue },
                { StartupBase.CountryOfOriginName, @"Germany" }
            });
            TrackingContext currentContext = TrackingContext.Current;
            currentContext.ShouldNotBeNull();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, @"/api/values");

            string tcHeader = TrackingContext.Serialize(currentContext).ByteArrayToBase64String();
            requestMessage.Headers.Add(TrackingContextHelper.TrackingContextKeyName, tcHeader);

            var response = await client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).ShouldBeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.ShouldNotBe(Guid.Empty);
            tc.OriginatorUtcTimestamp.ShouldBe(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.ShouldBe(4);
            tc.ExtraHeaders[headerKey].ShouldBe(headerValue);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].ShouldNotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].ShouldBe(@"Germany");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].ShouldNotBeNullOrWhiteSpace();

            TrackingContext.Current.ShouldNotBeNull();
            TrackingContext.Current.ShouldBeEquivalentTo(currentContext);

            List<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count.ShouldBe(10);

            callChainIds.All(x => x.Equals(tc.CallChainId.ToDashedString())).ShouldBeTrue();
        }

        [Fact]
        public async Task Startup_GivenTrackingContextWithMergeAndOverwriteMiddleware_WhenNoTrackingContextSent_ThenTrackingContextReturned()
        {
            var textWriterSink = new StringWriter();
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<StartupWithMergeAndOverwrite>();
                    webHost.ConfigureServices((services) => StartupBase.ConfigureServicesWithLogSink(
                        services,
                        textWriterSink,
                        $"{{{StartupBase.CountryOfOriginName}}}{textWriterSink.NewLine}"));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            TrackingContext.Current.ShouldBeNull();

            var response = await client.GetAsync(@"/api/values");
            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).ShouldBeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.ShouldNotBe(Guid.Empty);
            tc.OriginatorUtcTimestamp.ShouldBe(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.ShouldBe(3);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].ShouldNotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].ShouldBe(@"France");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].ShouldNotBeNullOrWhiteSpace();

            TrackingContext.Current.ShouldBeNull();

            List<string> countriesOfOrigin = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            countriesOfOrigin.Count.ShouldBe(10);

            countriesOfOrigin.All(x => x.Equals(tc.ExtraHeaders[StartupBase.CountryOfOriginName])).ShouldBeTrue();
        }

        [Fact]
        public async Task Startup_GivenTrackingContextWithMergeAndOverwriteMiddleware_WhenTrackingContextSent_ThenTrackingContextReturned()
        {
            var textWriterSink = new StringWriter();
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<StartupWithMergeAndOverwrite>();
                    webHost.ConfigureServices((services) => StartupBase.ConfigureServicesWithLogSink(
                        services,
                        textWriterSink,
                        $"{{{StartupBase.CountryOfOriginName}}}{textWriterSink.NewLine}"));
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            string headerKey = Guid.NewGuid().ToFlatString();
            string headerValue = Guid.NewGuid().ToFlatString();
            TrackingContext.NewCurrentIfEmpty(new Dictionary<string, string>()
            {
                { headerKey, headerValue },
                { StartupBase.CountryOfOriginName, @"Germany" }
            });
            TrackingContext currentContext = TrackingContext.Current;
            currentContext.ShouldNotBeNull();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, @"/api/values");

            string tcHeader = TrackingContext.Serialize(currentContext).ByteArrayToBase64String();
            requestMessage.Headers.Add(TrackingContextHelper.TrackingContextKeyName, tcHeader);

            var response = await client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).ShouldBeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.ShouldNotBe(Guid.Empty);
            tc.OriginatorUtcTimestamp.ShouldBe(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.ShouldBe(4);
            tc.ExtraHeaders[headerKey].ShouldBe(headerValue);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].ShouldNotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].ShouldBe(@"France");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].ShouldNotBeNullOrWhiteSpace();

            TrackingContext.Current.ShouldNotBeNull();
            TrackingContext.Current.ShouldBeEquivalentTo(currentContext);

            List<string> countriesOfOrigin = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            countriesOfOrigin.Count.ShouldBe(10);

            countriesOfOrigin.All(x => x.Equals(tc.ExtraHeaders[StartupBase.CountryOfOriginName])).ShouldBeTrue();
        }
    }
}
