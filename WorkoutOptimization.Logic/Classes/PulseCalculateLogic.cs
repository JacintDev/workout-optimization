using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using WorkoutOptimization.Logic.Interfaces;

namespace WorkoutOptimization.Logic.Classes
{
    public class PulseCalculateLogic : IDisposable, IPulseCalculateLogic
    {
        readonly InferenceSession _session;

        public PulseCalculateLogic()
        {
            string modelPath = @"C:\Users\kovac\source\repos\WorkoutOptimization\WorkoutOptimization.Data\Onxx\hr_model_tf2onnx.onnx";
            try
            {
                _session = new InferenceSession(modelPath);

            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing ONXX model. " + ex.Message);
            }
        }


        public float Calculate(float age, float restPulse)
        {
            // Input tensor: 1 sor, 2 oszlop (age, restPulse)
            var inputData = new DenseTensor<float>(new[] { age, restPulse }, new[] { 1, 2 });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputData)
            };

            using var results = _session.Run(inputs);
            float predicted = results.First().AsEnumerable<float>().First();

            

            return predicted;
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
