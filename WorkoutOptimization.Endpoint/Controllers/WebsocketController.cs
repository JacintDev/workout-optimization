using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebsocketController : ControllerBase
    {

        private readonly IGyroscopeDataLogic _gyroscopeDataLogic;
        private readonly ConcurrentQueue<GyroscopeDataDto> _queue;
        public WebsocketController(IGyroscopeDataLogic gyroscopeDataLogic, ConcurrentQueue<GyroscopeDataDto> queue)
        {
            this._gyroscopeDataLogic = gyroscopeDataLogic;
            this._queue = queue;
        }
        [HttpGet("connect")]
        public async Task<IActionResult> Connect()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {

                return BadRequest();
            }

            //var token= HttpContext.Request.Headers["Authorization"].ToString();
            //var tokenFromRequest = HttpContext.Request.Query["access_token"];
            ; 

            //Hozzáadjuk a bag-hez a socketet
            var socket= await HttpContext.WebSockets.AcceptWebSocketAsync();
            WebSocketHandler.AddClient(socket);

            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var res = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (res.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        break;
                    }

                    var msg = Encoding.UTF8.GetString(buffer, 0, res.Count);
                    GyroscopeDataDto gyD = JsonConvert.DeserializeObject<GyroscopeDataDto>(msg)!;

                    //_gyroscopeDataLogic.Create(gyD);
                    _queue.Enqueue(gyD);
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
            return new EmptyResult();



        }

        [HttpGet("start")]
        public async Task<IActionResult> Start()
        {
            await WebSocketHandler.BroadCastMessage("start");
            return Ok(new { message= "WebSocket: Training has been started!" });
        }
        [HttpGet("stop")]
        public async Task<IActionResult> Stop()
        {
            await WebSocketHandler.BroadCastMessage("stop");
            return Ok(new { message= "WebSocket: Training has been stopped!"});
        }


    }
}
