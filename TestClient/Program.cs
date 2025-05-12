using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorkoutOptimization.Models;

var uri = new Uri("ws://localhost:5135/Websocket/connect?access_token=1223231_asdasbadwweqeqwrtqdasddqwweq_dsadwqeqweqwrtwetew_");

using var client = new ClientWebSocket();
client.Options.SetRequestHeader("Authorization", "Bearer: BearerToken123");
await client.ConnectAsync(uri, CancellationToken.None);
Console.WriteLine("WebSocket connected!");

// Küldő folyamat példánya
Task? sendingTask = null;
CancellationTokenSource? sendingCts = null;

async Task StartSendingLoop()
{
    sendingCts = new CancellationTokenSource();

    try
    {
        while (!sendingCts.Token.IsCancellationRequested)
        {
            var input = new GyroscopeDataDto()
            {
                AccelX = 0,
                AccelY = 0,
                AccelZ = 0,
                GyrosX = 0,
                GyrosY = 0,
                GyrosZ = 0,
                TrainingId=4038
            };
            
            string json= JsonConvert.SerializeObject(input);
            var data = Encoding.UTF8.GetBytes(json);
            
            await client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"Küldve: {input}");

            await Task.Delay(100000, sendingCts.Token); // Várakozás megszakítható legyen
        }
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("Küldés leállítva.");
    }
}

// Várjuk a szervertől jövő üzeneteket
var receiveBuffer = new byte[1024];

while (client.State == WebSocketState.Open)
{
    var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
    var msg = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
    Console.WriteLine($"[Szerver]: {msg}");

    if (msg == "start")
    {
        if (sendingTask == null || sendingTask.IsCompleted)
        {
            Console.WriteLine("Küldés elindítása.");
            sendingTask = StartSendingLoop(); // nem await-eljük, háttérben fut
        }
    }
    else if (msg == "stop")
    {
        if (sendingCts != null)
        {
            Console.WriteLine("Küldés megállítása.");
            sendingCts.Cancel();
        }
    }
}
