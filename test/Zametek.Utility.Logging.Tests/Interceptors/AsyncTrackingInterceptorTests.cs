using Castle.DynamicProxy;
using Shouldly;
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

            TrackingContext.Current.ShouldBeNull();

            TrackingContext returnedTrackingContext = null;
            await proxy.ReturnAsync(() =>
            {
                returnedTrackingContext = TrackingContext.Current;
            });

            returnedTrackingContext.ShouldNotBeNull();
            TrackingContext.Current.ShouldBeNull();
        }

        [Fact]
        public async Task AsyncTrackingInterceptor_GivenTrackingContext_WhenReturnAsync_ThenSameTrackingContextReturned()
        {
            var instance = new TestTrackingService();
            var interceptor = new AsyncTrackingInterceptor();

            ITestTrackingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestTrackingService>(instance, interceptor.ToInterceptor());

            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            currentTrackingContext.ShouldNotBeNull();

            TrackingContext returnedTrackingContext = null;
            await proxy.ReturnAsync(() =>
            {
                returnedTrackingContext = TrackingContext.Current;
            });

            TrackingContext.Current.ShouldNotBeNull();
            TrackingContext.Current.CallChainId.ShouldBe(currentTrackingContext.CallChainId);
            TrackingContext.Current.OriginatorUtcTimestamp.ShouldBe(currentTrackingContext.OriginatorUtcTimestamp);

            returnedTrackingContext.ShouldNotBeNull();
            returnedTrackingContext.CallChainId.ShouldBe(currentTrackingContext.CallChainId);
            returnedTrackingContext.OriginatorUtcTimestamp.ShouldBe(currentTrackingContext.OriginatorUtcTimestamp);
        }

        [Fact]
        public async Task AsyncTrackingInterceptor_GivenNoTrackingContext_WhenReturnTrackingContextAsync_ThenNewTrackingContextReturned()
        {
            var instance = new TestTrackingService();
            var interceptor = new AsyncTrackingInterceptor();

            ITestTrackingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestTrackingService>(instance, interceptor.ToInterceptor());

            TrackingContext.Current.ShouldBeNull();

            TrackingContext returnedTrackingContext = await proxy.ReturnTrackingContextAsync();

            returnedTrackingContext.ShouldNotBeNull();
            TrackingContext.Current.ShouldBeNull();
        }

        [Fact]
        public async Task AsyncTrackingInterceptor_GivenTrackingContext_WhenReturnTrackingContextAsync_ThenSameTrackingContextReturned()
        {
            var instance = new TestTrackingService();
            var interceptor = new AsyncTrackingInterceptor();

            ITestTrackingService proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITestTrackingService>(instance, interceptor.ToInterceptor());

            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            currentTrackingContext.ShouldNotBeNull();

            TrackingContext returnedTrackingContext = await proxy.ReturnTrackingContextAsync();

            TrackingContext.Current.ShouldNotBeNull();
            TrackingContext.Current.CallChainId.ShouldBe(currentTrackingContext.CallChainId);
            TrackingContext.Current.OriginatorUtcTimestamp.ShouldBe(currentTrackingContext.OriginatorUtcTimestamp);

            returnedTrackingContext.ShouldNotBeNull();
            returnedTrackingContext.CallChainId.ShouldBe(currentTrackingContext.CallChainId);
            returnedTrackingContext.OriginatorUtcTimestamp.ShouldBe(currentTrackingContext.OriginatorUtcTimestamp);
        }
    }
}
