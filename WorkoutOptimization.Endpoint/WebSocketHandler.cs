using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace WorkoutOptimization.Endpoint
{
    public static class WebSocketHandler
    {
        private static ConcurrentBag<WebSocket> _sockets = new();

        public static void AddClient(WebSocket socket)
        {
            _sockets.Add(socket);
        }

        public static async Task BroadCastMessage(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment= new ArraySegment<byte>(buffer);
            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
