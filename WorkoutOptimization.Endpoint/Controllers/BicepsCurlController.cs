using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    
    public class BicepsCurlController : ControllerBase
    {
        readonly IBicepsCurlLogic _logic;

        public BicepsCurlController(IBicepsCurlLogic logic)
        {
            _logic = logic;
        }

        [HttpPost]
        public bool Get(float[][] inputData)
        {
            // Ha ragaszkodsz a 3D tömbhöz, konvertáld át itt pl.:
            float[,,] converted = new float[1, 13, 6];
            for (int j = 0; j < 13; j++)
            {
                for (int k = 0; k < 6; k++)
                {
                    converted[0, j, k] = inputData[j][k];
                }
            }
            return _logic.DataValidation(converted);
        }

    }
}
