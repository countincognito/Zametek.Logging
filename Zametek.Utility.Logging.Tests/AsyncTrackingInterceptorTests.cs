using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.Tests
{
    [TestClass]
    public class AsyncTrackingInterceptorTests
    {
        private static readonly IProxyGenerator s_ProxyGenerator = new ProxyGenerator();

        [TestMethod]
        public async Task AsyncTrackingInterceptor_ReturnAsyncWithoutCurrent_NewTrackingContextReturned()
        {
            var instance = new Test();
            var interceptor = new AsyncTrackingInterceptor();

            ITest proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITest>(instance, interceptor.ToInterceptor());

            Assert.IsNull(TrackingContext.Current);

            TrackingContext returnedTrackingContext = null;
            await proxy.ReturnAsync(() =>
            {
                returnedTrackingContext = TrackingContext.Current;
            });

            Assert.IsNotNull(returnedTrackingContext);
            Assert.IsNull(TrackingContext.Current);
        }

        [TestMethod]
        public async Task AsyncTrackingInterceptor_ReturnAsyncWithCurrent_SameTrackingContextReturned()
        {
            var instance = new Test();
            var interceptor = new AsyncTrackingInterceptor();

            ITest proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITest>(instance, interceptor.ToInterceptor());

            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            Assert.IsNotNull(currentTrackingContext);

            TrackingContext returnedTrackingContext = null;
            await proxy.ReturnAsync(() =>
            {
                returnedTrackingContext = TrackingContext.Current;
            });

            Assert.IsNotNull(TrackingContext.Current);
            Assert.AreEqual(TrackingContext.Current.CallChainId, currentTrackingContext.CallChainId);
            Assert.AreEqual(TrackingContext.Current.OriginatorUtcTimestamp, currentTrackingContext.OriginatorUtcTimestamp);

            Assert.IsNotNull(returnedTrackingContext);
            Assert.AreEqual(currentTrackingContext.CallChainId, returnedTrackingContext.CallChainId);
            Assert.AreEqual(currentTrackingContext.OriginatorUtcTimestamp, returnedTrackingContext.OriginatorUtcTimestamp);
        }

        [TestMethod]
        public async Task AsyncTrackingInterceptor_ReturnTrackingContextAsyncWithoutCurrent_NewTrackingContextReturned()
        {
            var instance = new Test();
            var interceptor = new AsyncTrackingInterceptor();

            ITest proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITest>(instance, interceptor.ToInterceptor());

            Assert.IsNull(TrackingContext.Current);

            TrackingContext returnedTrackingContext = await proxy.ReturnTrackingContextAsync();

            Assert.IsNotNull(returnedTrackingContext);
            Assert.IsNull(TrackingContext.Current);
        }

        [TestMethod]
        public async Task AsyncTrackingInterceptor_ReturnTrackingContextAsyncWithCurrent_SameTrackingContextReturned()
        {
            var instance = new Test();
            var interceptor = new AsyncTrackingInterceptor();

            ITest proxy = s_ProxyGenerator.CreateInterfaceProxyWithTargetInterface<ITest>(instance, interceptor.ToInterceptor());

            TrackingContext.NewCurrent();
            TrackingContext currentTrackingContext = TrackingContext.Current;

            Assert.IsNotNull(currentTrackingContext);

            TrackingContext returnedTrackingContext = await proxy.ReturnTrackingContextAsync();

            Assert.IsNotNull(TrackingContext.Current);
            Assert.AreEqual(TrackingContext.Current.CallChainId, currentTrackingContext.CallChainId);
            Assert.AreEqual(TrackingContext.Current.OriginatorUtcTimestamp, currentTrackingContext.OriginatorUtcTimestamp);

            Assert.IsNotNull(returnedTrackingContext);
            Assert.AreEqual(currentTrackingContext.CallChainId, returnedTrackingContext.CallChainId);
            Assert.AreEqual(currentTrackingContext.OriginatorUtcTimestamp, returnedTrackingContext.OriginatorUtcTimestamp);
        }
    }
}
