using System;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.Tests
{
    public interface ITestTrackingService
    {
        Task ReturnAsync(Action action);
        Task<TrackingContext> ReturnTrackingContextAsync();
    }
}
