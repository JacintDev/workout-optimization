using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Logic
{
    public class BicepsCurlLogic : IBicepsCurlLogic
    {
        public bool DataValidation(float[,,] inputData)
        {
            var inputData2 = new float[1, 13, 6]
            {
                {
                    { -15.1626f, 21.75574f, -34.74697f, 0.475283f, 0.904518f, 0.0f },
                    { -11.97223f, 7.577221f, -19.76566f, 0.458235f, 0.91932f, 0.0f },
                    { -5.610339f, -24.16377f, 29.39814f, 0.483227f, 0.918184f, 0.0f },
                    { -4.72645f, -27.82508f, 71.77637f, 0.602199f, 0.813601f, 0.0f },
                    { -2.811513f, -4.320055f, 18.2261f, 0.652763f, 0.722175f, 0.0f },
                    { 4.678234f, 17.62824f, -24.00697f, 0.687654f, 0.740585f, 0.0f },
                    { 11.79296f, 38.18948f, -41.21156f, 0.633589f, 0.795191f, 0.0f },
                    { 6.716588f, 28.46723f, -47.00135f, 0.538645f, 0.85884f, 0.0f },
                    { 17.54955f, 31.7818f, 11.92742f, 0.464947f, 0.912424f, 0.0f },
                    { 24.08159f, 17.65937f, 46.49049f, 0.524748f, 0.895147f, 0.0f },
                    { 17.84534f, -7.601378f, 75.90823f, 0.62771f, 0.80402f, 0.0f },
                    { 0.0f, 4.495685f, -1.952398f, 0.697059f, 0.685176f, 0.0f },
                    { -11.62881f, 7.719298f, -54.64145f, 0.578818f, 0.812271f, 0.0f }
                }
            };

            // Normalizálás a modell bemenete előtt
            float[] data_min_ = new float[] { -162.8546f, -346.2087f, -199.0442f, -0.1812390f, -0.26209f, -0.8285080f };
            float[] data_range_ = new float[] { 283.8906f, 561.3296f, 411.5715f, 1.225877f, 1.336848f, 1.501098f };

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        inputData[i, j, k] = (inputData[i, j, k] - data_min_[k]) / data_range_[k];
                    }
                }
            }
            // Flatten inputData 3D -> 1D
            float[] inputFlat = new float[1 * 13 * 6];
            int idx = 0;
            for (int i = 0; i < 1; i++)
                for (int j = 0; j < 13; j++)
                    for (int k = 0; k < 6; k++)
                        inputFlat[idx++] = inputData[i, j, k];

            // Készíts DenseTensor-t az inputból
            var inputTensor = new DenseTensor<float>(inputFlat, new int[] { 1, 13, 6 });
            string modelPath = @"C:\Users\kovac\source\repos\WorkoutOptimization\WorkoutOptimization.Data\best_model.onnx";

            using var session = new InferenceSession(modelPath);
            string inputName = session.InputMetadata.Keys.First();

            // Az inputot NamedOnnxValue-ként kell átadni a modellt futtatásához
            var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
        };

            // Lefuttatjuk az előrejelzést
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);

            // Kivesszük az eredményt (feltételezve 1 output)
            var outputTensor = results.First().AsTensor<float>();

            // Kiírjuk az outputot
            Console.WriteLine("Output:");
            foreach (var v in outputTensor.ToArray())
            {
                if (v > 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;


        }
    }
}
