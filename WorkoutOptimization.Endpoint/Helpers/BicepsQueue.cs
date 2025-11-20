using System.Collections.Concurrent;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Endpoint.Helpers
{
    public class BicepsQueue: ConcurrentQueue<GyroscopeDataDto>
    {
    }
}
