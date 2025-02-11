using Castle.DynamicProxy;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Zametek.Utility.Logging.Tests
{
    public class AsyncTrackingInterceptorTests
    {
        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();

        [Fact]
        public async Task AsyncTrackingInterceptor_GivenNoTrackingContext_WhenReturnAsync_ThenNewTrackingContextReturned()
        {
            var instance = new TestTrackingService();
            var interceptor = new AsyncTrackingInterceptor();

            ITestTrackingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestTrackingService>(instance, interceptor.ToInterceptor());

            TrackingContext.Current.Should().BeNull();

            TrackingContext returnedTrackingContext = null;
            await proxy.ReturnAsync(() =>
            {
                returnedTrackingContext = TrackingContext.Current;
            });

            returnedTrackingContext.Should().NotBeNull();
            TrackingContext.Current.Should().BeNull();
        }

        [Fact]
        public async Task AsyncTrackingInterceptor_GivenTrackingContext_WhenReturnAsync_ThenSameTrackingContextReturned()
        {
            var instance = new TestTrackingService();
            var interceptor = new AsyncTrackingInterceptor();

            ITestTrackingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestTrackingService>(instance, interceptor.ToInterceptor());

            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            currentTrackingContext.Should().NotBeNull();

            TrackingContext returnedTrackingContext = null;
            await proxy.ReturnAsync(() =>
            {
                returnedTrackingContext = TrackingContext.Current;
            });

            TrackingContext.Current.Should().NotBeNull();
            TrackingContext.Current.CallChainId.Should().Be(currentTrackingContext.CallChainId);
            TrackingContext.Current.OriginatorUtcTimestamp.Should().Be(currentTrackingContext.OriginatorUtcTimestamp);

            returnedTrackingContext.Should().NotBeNull();
            returnedTrackingContext.CallChainId.Should().Be(currentTrackingContext.CallChainId);
            returnedTrackingContext.OriginatorUtcTimestamp.Should().Be(currentTrackingContext.OriginatorUtcTimestamp);
        }

        [Fact]
        public async Task AsyncTrackingInterceptor_GivenNoTrackingContext_WhenReturnTrackingContextAsync_ThenNewTrackingContextReturned()
        {
            var instance = new TestTrackingService();
            var interceptor = new AsyncTrackingInterceptor();

            ITestTrackingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestTrackingService>(instance, interceptor.ToInterceptor());

            TrackingContext.Current.Should().BeNull();

            TrackingContext returnedTrackingContext = await proxy.ReturnTrackingContextAsync();

            returnedTrackingContext.Should().NotBeNull();
            TrackingContext.Current.Should().BeNull();
        }

        [Fact]
        public async Task AsyncTrackingInterceptor_GivenTrackingContext_WhenReturnTrackingContextAsync_ThenSameTrackingContextReturned()
        {
            var instance = new TestTrackingService();
            var interceptor = new AsyncTrackingInterceptor();

            ITestTrackingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestTrackingService>(instance, interceptor.ToInterceptor());

            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            currentTrackingContext.Should().NotBeNull();

            TrackingContext returnedTrackingContext = await proxy.ReturnTrackingContextAsync();

            TrackingContext.Current.Should().NotBeNull();
            TrackingContext.Current.CallChainId.Should().Be(currentTrackingContext.CallChainId);
            TrackingContext.Current.OriginatorUtcTimestamp.Should().Be(currentTrackingContext.OriginatorUtcTimestamp);

            returnedTrackingContext.Should().NotBeNull();
            returnedTrackingContext.CallChainId.Should().Be(currentTrackingContext.CallChainId);
            returnedTrackingContext.OriginatorUtcTimestamp.Should().Be(currentTrackingContext.OriginatorUtcTimestamp);
        }
    }
}
