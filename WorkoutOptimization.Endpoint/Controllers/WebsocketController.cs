using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebsocketController : ControllerBase
    {
        [HttpGet("connect")]
        public async Task<IActionResult> Connect()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {

                return BadRequest();
            }

            var token= HttpContext.Request.Headers["Authorization"].ToString();
            var tokenFromRequest = HttpContext.Request.Query["access_token"];
            ; 

            //Hozzáadjuk a bag-hez a socketet
            var socket= await HttpContext.WebSockets.AcceptWebSocketAsync();
            WebSocketHandler.AddClient(socket);

            var buffer= new byte[1024];
            while (socket.State == WebSocketState.Open) { 
                var res= await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var msg = Encoding.UTF8.GetString(buffer, 0, res.Count);
                Console.WriteLine($"[Client]: {msg}");
            }
            return new EmptyResult();



        }

        [HttpGet("start")]
        public async Task<IActionResult> Start()
        {
            await WebSocketHandler.BroadCastMessage("start");
            return Ok();
        }
        [HttpGet("stop")]
        public async Task<IActionResult> Stop()
        {
            await WebSocketHandler.BroadCastMessage("stop");
            return Ok();
        }


    }
}
