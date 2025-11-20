using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Logic.Classes
{
    public class ShoulderPressLogic : IShoulderPressLogic
    {
        private readonly IRepository<MlModel> _repository;

        private static readonly object _initLock = new();
        private static bool _initialized = false;

        private static InferenceSession _session;
        private static float[] _dataMin;
        private static float[] _dataRange;
        private string ModelName { get; } = "ShoulderPress_LSTM";

        public ShoulderPressLogic(IRepository<MlModel> repository)
        {
            _repository = repository;
            EnsureModelLoaded();
        }

        /// <summary>
        /// Gondoskodik róla, hogy a modell és a normalizáló vektorok
        /// csak egyszer töltődjenek be (thread-safe).
        /// </summary>
        private void EnsureModelLoaded()
        {
            if (_initialized)
                return;

            lock (_initLock)
            {
                if (_initialized)
                    return;

                
                //_dataMin = new float[]
                //{
                //    -162.8546f, -346.2087f, -199.0442f, -0.1812390f, -0.26209f, -0.8285080f
                //}
                //;
                //_dataMin = _repository.Read(1004).DataMin.Split(' ').Select(x=> float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                _dataMin= _repository.ReadAll().Where(x=>x.Name.Trim() == ModelName).FirstOrDefault()
                    .DataMin.Split(' ').Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                //_dataRange = new float[]
                //{
                //    283.8906f, 561.3296f, 411.5715f, 1.225877f, 1.336848f, 1.501098f
                //};
                //_dataRange = _repository.Read(1004).DataRange.Split(' ').Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                _dataRange = _repository.ReadAll().Where(x => x.Name.Trim() == ModelName).FirstOrDefault()
                   .DataRange.Split(' ').Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();



                byte[] model = _repository.ReadAll().Where(x => x.Name.Trim() == ModelName).FirstOrDefault().ModelData;
                _session = new InferenceSession(model);

                _initialized = true;
            }
        }

        public bool DataValidation(float[,,] inputData)
        {
            // Biztonság kedvéért – ha valahonnan úgy hívnád, hogy még nincs inicializálva
            EnsureModelLoaded();

            // 1) Normalizálás
            // Feltételezzük, hogy inputData mérete [1, 13, 6]
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        inputData[i, j, k] = (inputData[i, j, k] - _dataMin[k]) / _dataRange[k];
                    }
                }
            }

            // 2) Flatten 3D -> 1D
            float[] inputFlat = new float[1 * 13 * 6];
            int idx = 0;
            for (int i = 0; i < 1; i++)
                for (int j = 0; j < 13; j++)
                    for (int k = 0; k < 6; k++)
                        inputFlat[idx++] = inputData[i, j, k];

            // 3) Tensor készítése
            var inputTensor = new DenseTensor<float>(inputFlat, new int[] { 1, 13, 6 });

            string inputName = _session.InputMetadata.Keys.First();

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
            };

            // 4) Inference – a session már statikus, nem jön létre minden hívásnál újra
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _session.Run(inputs);

            var outputTensor = results.First().AsTensor<float>();
            var outputs = outputTensor.ToArray();

            // 5) Egyszerű threshold-os döntés (0.5)
            // (Ha valójában több logit / softmax output van, ezt igény szerint módosíthatod.)
            if (outputs.Length == 0)
                return false;

            // Pl. binary kimenetnél:
            return outputs[0] > 0.5f;
        }
    }
}
