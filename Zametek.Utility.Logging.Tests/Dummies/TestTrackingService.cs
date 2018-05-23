using System;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.Tests
{
    public class TestTrackingService
        : ITestTrackingService
    {
        public async Task ReturnAsync(Action action)
        {
            action.Invoke();
            await Task.CompletedTask;
        }

        public async Task<TrackingContext> ReturnTrackingContextAsync()
        {
            return await Task.FromResult(TrackingContext.Current);
        }
    }
}
