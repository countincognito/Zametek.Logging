using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Destructurama;
using System.IO;

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

            TrackingContext.Current.Should().BeNull();

            var response = await client.GetAsync(@"/api/values");
            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).Should().BeFalse();

            TrackingContext.Current.Should().BeNull();

            IList<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count().Should().Be(10);

            //callChainIds.All(x => x.Equals(tc.CallChainId.ToDashedString())).Should().BeTrue();
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
            currentContext.Should().NotBeNull();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, @"/api/values");

            string tcHeader = TrackingContext.Serialize(currentContext).ByteArrayToBase64String();
            requestMessage.Headers.Add(TrackingContextHelper.TrackingContextKeyName, tcHeader);

            var response = await client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).Should().BeFalse();

            TrackingContext.Current.Should().NotBeNull();
            TrackingContext.Current.Should().BeEquivalentTo(currentContext);

            IList<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count().Should().Be(10);

            // The call chain ID should be different on the server side.
            callChainIds.All(x => !x.Equals(currentContext.CallChainId.ToDashedString())).Should().BeTrue();
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

            TrackingContext.Current.Should().BeNull();

            var response = await client.GetAsync(@"/api/values");
            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).Should().BeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.Should().NotBeEmpty();
            tc.OriginatorUtcTimestamp.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.Should().Be(3);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].Should().NotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].Should().Be(@"UK");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].Should().NotBeNullOrWhiteSpace();

            TrackingContext.Current.Should().BeNull();

            IList<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count().Should().Be(10);

            callChainIds.All(x => x.Equals(tc.CallChainId.ToDashedString())).Should().BeTrue();
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
            currentContext.Should().NotBeNull();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, @"/api/values");

            string tcHeader = TrackingContext.Serialize(currentContext).ByteArrayToBase64String();
            requestMessage.Headers.Add(TrackingContextHelper.TrackingContextKeyName, tcHeader);

            var response = await client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).Should().BeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.Should().NotBeEmpty();
            tc.OriginatorUtcTimestamp.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.Should().Be(4);
            tc.ExtraHeaders[headerKey].Should().Be(headerValue);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].Should().NotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].Should().Be(@"Germany");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].Should().NotBeNullOrWhiteSpace();

            TrackingContext.Current.Should().NotBeNull();
            TrackingContext.Current.Should().BeEquivalentTo(currentContext);

            IList<string> callChainIds = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            callChainIds.Count().Should().Be(10);

            callChainIds.All(x => x.Equals(tc.CallChainId.ToDashedString())).Should().BeTrue();
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

            TrackingContext.Current.Should().BeNull();

            var response = await client.GetAsync(@"/api/values");
            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).Should().BeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.Should().NotBeEmpty();
            tc.OriginatorUtcTimestamp.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.Should().Be(3);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].Should().NotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].Should().Be(@"France");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].Should().NotBeNullOrWhiteSpace();

            TrackingContext.Current.Should().BeNull();

            IList<string> countriesOfOrigin = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            countriesOfOrigin.Count().Should().Be(10);

            countriesOfOrigin.All(x => x.Equals(tc.ExtraHeaders[StartupBase.CountryOfOriginName])).Should().BeTrue();
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
            currentContext.Should().NotBeNull();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, @"/api/values");

            string tcHeader = TrackingContext.Serialize(currentContext).ByteArrayToBase64String();
            requestMessage.Headers.Add(TrackingContextHelper.TrackingContextKeyName, tcHeader);

            var response = await client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();
            response.Headers.TryGetValues(TrackingContextHelper.TrackingContextKeyName, out IEnumerable<string> values).Should().BeTrue();

            string tcBase64 = values.First();
            TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());

            tc.CallChainId.Should().NotBeEmpty();
            tc.OriginatorUtcTimestamp.Should().BeCloseTo(DateTime.UtcNow, new TimeSpan(0, 0, 1));
            tc.ExtraHeaders.Count.Should().Be(4);
            tc.ExtraHeaders[headerKey].Should().Be(headerValue);
            tc.ExtraHeaders[StartupBase.TraceIdentifierName].Should().NotBeNullOrWhiteSpace();
            tc.ExtraHeaders[StartupBase.CountryOfOriginName].Should().Be(@"France");
            tc.ExtraHeaders[StartupBase.RandomStringGeneratedWitEachCallName].Should().NotBeNullOrWhiteSpace();

            TrackingContext.Current.Should().NotBeNull();
            TrackingContext.Current.Should().BeEquivalentTo(currentContext);

            IList<string> countriesOfOrigin = textWriterSink
                .ToString()
                .Split(textWriterSink.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            countriesOfOrigin.Count().Should().Be(10);

            countriesOfOrigin.All(x => x.Equals(tc.ExtraHeaders[StartupBase.CountryOfOriginName])).Should().BeTrue();
        }
    }
}
