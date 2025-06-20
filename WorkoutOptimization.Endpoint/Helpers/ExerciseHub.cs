using Microsoft.AspNetCore.SignalR;

namespace WorkoutOptimization.Endpoint.Helpers
{
    public class ExerciseHub : Hub
    {
        public async Task SendPrediction(string message)
        {
            await Clients.All.SendAsync("ReceivePrediction", message);
        }
    }
}
