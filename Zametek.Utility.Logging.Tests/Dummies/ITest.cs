using System;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.Tests
{
    public interface ITest
    {
        Task ReturnAsync(Action action);
        Task<TrackingContext> ReturnTrackingContextAsync();
    }
}
