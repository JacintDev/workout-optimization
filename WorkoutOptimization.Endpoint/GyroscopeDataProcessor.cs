
using System.Collections.Concurrent;
using WorkoutOptimization.Repository;
using ScottPlot;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using WorkoutOptimization.Endpoint.Helpers;
using WorkoutOptimization.Repository.Migrations;
using WorkoutOptimization.Models.Models;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;

namespace WorkoutOptimization.Endpoint
{
   

    public class GyroscopeDataProcessor : BackgroundService
    {
        private readonly ConcurrentQueue<GyroscopeDataDto> _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ExerciseHub> _hubContext;

        // Tengelyválasztáshoz:
        private volatile Axis _repAxis;
        private Func<GyroscopeDataDto, double> _getAxis;
        private int? _lastTrainingId = null;

        public GyroscopeDataProcessor(
            ConcurrentQueue<GyroscopeDataDto> queue,
            IServiceScopeFactory scopeFactory,
            IHubContext<ExerciseHub> hubContext,
            Axis repAxis = Axis.GyrosZ // alapból a régi viselkedés
        )
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;

            SetAxis(repAxis);
        }

        /// <summary>
        /// Futás közben is átállíthatod a detektálás tengelyét.
        /// </summary>
        public void SetAxis(Axis axis)
        {
            _repAxis = axis;
            _getAxis = axis switch
            {
                Axis.GyrosX => d => d.GyrosX,
                Axis.GyrosY => d => d.GyrosY,
                Axis.GyrosZ => d => d.GyrosZ,
                Axis.AccelX => d => d.AccelX,
                Axis.AccelY => d => d.AccelY,
                Axis.AccelZ => d => d.AccelZ,
                _ => d => d.GyrosZ
            };

            Console.WriteLine($"[i] Ismétlés detektálás tengelye: {_repAxis}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<GyroscopeDataDto> repBuffer = new();
            Queue<double> slidingWindow = new(); // kiválasztott tengely értékei
            Queue<GyroscopeDataDto> rawWindow = new(); // teljes objektumok

            const int windowSize = 7;

            while (!stoppingToken.IsCancellationRequested)
            {
                while (_queue.TryDequeue(out var data))
                {

                    if (data.TrainingId != null)
                    {
                        int currentId= (int)data.TrainingId;
                        if(currentId!=_lastTrainingId)
                        {
                            _lastTrainingId= currentId;
                            using var scope = _scopeFactory.CreateScope();
                            var trainingRepo = scope.ServiceProvider.GetRequiredService<IRepository<Training>>();
                            var training = trainingRepo.Read((int)data.TrainingId);
                            SetAxis(training.Axis);

                        }

                        
     
                    }

                    repBuffer.Add(data);
                    rawWindow.Enqueue(data);
                    slidingWindow.Enqueue(_getAxis(data)); // <<< ITT már a választott tengely megy

                    // csak akkor vizsgáljuk, ha megvan a teljes ablak
                    if (slidingWindow.Count == windowSize)
                    {
                        var values = slidingWindow.ToArray();
                        int middleIndex = windowSize / 2;
                        double middle = values[middleIndex];

                        // Minimum detektálás (ha maximum kellene, elég a relációt megfordítani)
                        bool isMinimum = true;
                        for (int i = 0; i < windowSize; i++)
                        {
                            if (i == middleIndex) continue;
                            if (middle >= values[i])
                            {
                                isMinimum = false;
                                break;
                            }
                        }

                        if (isMinimum)
                        {
                            Console.WriteLine($"[✓] Ismétlés detektálva ({_repAxis} peak: {middle:F2})");

                            // A repBuffer tartalmazza az eddigi adatokat, de ne adjuk át a peak utáni 3-at
                            int cutIndex = repBuffer.Count - (windowSize - 3);
                            var repetition = repBuffer.Take(cutIndex).ToList(); // csak a peak-ig
                            await ProcessBatchAsync(repetition);

                            // Maradék visszatöltése
                            repBuffer = repBuffer.Skip(cutIndex).ToList();

                            // Sliding window újratöltése a megmaradt elemek tengely-értékeivel
                            var remainingAxisVals = repBuffer.Select(x => _getAxis(x)).ToList();
                            slidingWindow.Clear();
                            foreach (var val in remainingAxisVals)
                                slidingWindow.Enqueue(val);

                            rawWindow.Clear();
                            foreach (var item in repBuffer)
                                rawWindow.Enqueue(item);
                        }
                        else
                        {
                            // csúsztatjuk az ablakot
                            slidingWindow.Dequeue();
                            rawWindow.Dequeue();
                        }
                    }
                }

                await Task.Delay(10, stoppingToken);
            }
        }



        private async Task ProcessBatchAsync(List<GyroscopeDataDto> batch)
        {
            int targetLength = 13; // fix hossz

            double[] gyrosX = batch.Select(d => (double)d.GyrosX).ToArray();
            double[] gyrosY = batch.Select(d => (double)d.GyrosY).ToArray();
            double[] gyrosZ = batch.Select(d => (double)d.GyrosZ).ToArray();
            double[] accelX = batch.Select(d => (double)d.AccelX).ToArray();
            double[] accelY = batch.Select(d => (double)d.AccelY).ToArray();
            double[] accelZ = batch.Select(d => (double)d.AccelZ).ToArray();

            // Normalizált (interpolált) adatok
            double[] normGyrosX = NormalizeLength(gyrosX, targetLength);
            double[] normGyrosY = NormalizeLength(gyrosY, targetLength);
            double[] normGyrosZ = NormalizeLength(gyrosZ, targetLength);
            double[] normAccelX = NormalizeLength(accelX, targetLength);
            double[] normAccelY = NormalizeLength(accelY, targetLength);
            double[] normAccelZ = NormalizeLength(accelZ, targetLength);

            // Eredeti indexek (0 ... batch.Count-1)
            double[] origIndex = Enumerable.Range(0, batch.Count).Select(i => (double)i).ToArray();

            // Normalizált indexek (0 ... targetLength-1)
            double[] normIndex = Enumerable.Range(0, targetLength).Select(i => (double)i).ToArray();

            var plt = new ScottPlot.Plot(800, 600);

            // Eredeti adatok scatter
            plt.AddScatter(origIndex, gyrosX, label: "Gyro X (orig)", color: System.Drawing.Color.Red, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
            plt.AddScatter(origIndex, gyrosY, label: "Gyro Y (orig)", color: System.Drawing.Color.Green, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
            plt.AddScatter(origIndex, gyrosZ, label: "Gyro Z (orig)", color: System.Drawing.Color.Blue, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);

            plt.AddScatter(origIndex, accelX, label: "Accel X (orig)", color: System.Drawing.Color.Orange, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
            plt.AddScatter(origIndex, accelY, label: "Accel Y (orig)", color: System.Drawing.Color.Purple, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
            plt.AddScatter(origIndex, accelZ, label: "Accel Z (orig)", color: System.Drawing.Color.Brown, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);

            // Normalizált adatok scatter - például vonallal, más színnel
            plt.AddScatter(normIndex, normGyrosX, label: "Gyro X (norm)", color: System.Drawing.Color.Red, lineWidth: 2, markerSize: 0, lineStyle: ScottPlot.LineStyle.Solid);
            plt.AddScatter(normIndex, normGyrosY, label: "Gyro Y (norm)", color: System.Drawing.Color.Green, lineWidth: 2, markerSize: 0, lineStyle: ScottPlot.LineStyle.Solid);
            plt.AddScatter(normIndex, normGyrosZ, label: "Gyro Z (norm)", color: System.Drawing.Color.Blue, lineWidth: 2, markerSize: 0, lineStyle: ScottPlot.LineStyle.Solid);

            plt.AddScatter(normIndex, normAccelX, label: "Accel X (norm)", color: System.Drawing.Color.Orange, lineWidth: 2, markerSize: 0, lineStyle: ScottPlot.LineStyle.Solid);
            plt.AddScatter(normIndex, normAccelY, label: "Accel Y (norm)", color: System.Drawing.Color.Purple, lineWidth: 2, markerSize: 0, lineStyle: ScottPlot.LineStyle.Solid);
            plt.AddScatter(normIndex, normAccelZ, label: "Accel Z (norm)", color: System.Drawing.Color.Brown, lineWidth: 2, markerSize: 0, lineStyle: ScottPlot.LineStyle.Solid);

            plt.Legend(location: ScottPlot.Alignment.UpperLeft);
            plt.Title("Gyroscope & Accelerometer Data (Original vs Normalized)");
            plt.XLabel("Sample Index");
            plt.YLabel("Value");

            string path = $"plot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            plt.SaveFig(path);
            Console.WriteLine($"[✓] Grafikon elmentve: {path}");

            // Normalizált batch létrehozása (ahogy eddig)
            var normalizedBatch = new List<GyroscopeDataDto>();
            for (int i = 0; i < targetLength; i++)
            {
                normalizedBatch.Add(new GyroscopeDataDto
                {
                    GyrosX = (float)normGyrosX[i],
                    GyrosY = (float)normGyrosY[i],
                    GyrosZ = (float)normGyrosZ[i],
                    AccelX = (float)normAccelX[i],
                    AccelY = (float)normAccelY[i],
                    AccelZ = (float)normAccelZ[i],
                    // Egyéb mezők, ha vannak
                });
            }

            //var json = JsonSerializer.Serialize(normalizedBatch);
            //File.WriteAllText("json_normalized.json", json);

            var filePath = "json_normalized.json";
            List<GyroscopeDataDto> newItem = normalizedBatch; // ez az új objektumlista

            List<GyroscopeDataDto> items = new();

            // Ha már létezik a fájl, beolvassuk a meglévő listát
            if (File.Exists(filePath))
            {
                var existingJson = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                    items = JsonSerializer.Deserialize<List<GyroscopeDataDto>>(existingJson) ?? new List<GyroscopeDataDto>();
            }

            // Hozzáadjuk az új elemeket a meglévőkhöz
            items.AddRange(newItem);

            // Visszaírjuk a fájlt
            var newJson = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, newJson);

            //using var scope = _scopeFactory.CreateScope();
            //var logic = scope.ServiceProvider.GetRequiredService<IGyroscopeDataLogic>();
            //foreach (var data in normalizedBatch)
            //{
            //    logic.Create(data);
            //}
            float[,,] converted = new float[1, 13, 6];

            for (int i = 0; i < normalizedBatch.Count; i++)
            {
                var d = normalizedBatch[i];
                converted[0, i, 0] = d.GyrosX;
                converted[0, i, 1] = d.GyrosY;
                converted[0, i, 2] = d.GyrosZ;
                converted[0, i, 3] = d.AccelX;
                converted[0, i, 4] = d.AccelY;
                converted[0, i, 5] = d.AccelZ;
            }
            //Send to data validation
            using (var scope = _scopeFactory.CreateScope())
            {
                var bicepsLogic = scope.ServiceProvider.GetRequiredService<IBicepsCurlLogic>();
                var exerciseLogic = scope.ServiceProvider.GetRequiredService<IExerciseResultLogic>();

                var res = bicepsLogic.DataValidation(converted);
                AppendLabeledSequenceToJson(normalizedBatch, true);
                string message = res ? "Helyes" : "Helytelen";
                var trainingId = batch.First().TrainingId;

                var exerciseResult = new ExerciseResultCreateModel()
                {
                    IsCorrect = res,
                    TrainingId = trainingId == null ? 0 : (int)trainingId
                };

                await _hubContext.Clients.All.SendAsync("ReceivePrediction", message);
                await exerciseLogic.CreateExerciseResult(exerciseResult);
            }


        }


        public static double[] NormalizeLength(double[] originalValues, int targetLength)
        {
            int originalLength = originalValues.Length;
            double[] result = new double[targetLength];

            if (targetLength == originalLength)
                return (double[])originalValues.Clone();

            for (int i = 0; i < targetLength; i++)
            {
                double pos = (originalLength - 1) * i / (double)(targetLength - 1);
                int leftIndex = (int)Math.Floor(pos);
                int rightIndex = Math.Min(leftIndex + 1, originalLength - 1);
                double fraction = pos - leftIndex;

                result[i] = originalValues[leftIndex] * (1 - fraction) + originalValues[rightIndex] * fraction;
            }

            return result;
        }
        public static double[] DownsampleWithFixedEnds(double[] originalValues, int targetLength)
        {
            int originalLength = originalValues.Length;
            if (targetLength >= originalLength) return (double[])originalValues.Clone();

            double[] result = new double[targetLength];
            result[0] = originalValues[0];
            result[targetLength - 1] = originalValues[originalLength - 1];

            double step = (originalLength - 2) / (double)(targetLength - 2);
            for (int i = 1; i < targetLength - 1; i++)
            {
                int idx = (int)Math.Round(1 + step * (i - 1));
                result[i] = originalValues[idx];
            }

            return result;
        }

        public static double[] LinearInterpolate(double[] originalValues, int targetLength)
        {
            int originalLength = originalValues.Length;
            double[] result = new double[targetLength];

            if (targetLength == originalLength)
                return (double[])originalValues.Clone();

            for (int i = 0; i < targetLength; i++)
            {
                double pos = (originalLength - 1) * i / (double)(targetLength - 1);
                int leftIndex = (int)Math.Floor(pos);
                int rightIndex = Math.Min(leftIndex + 1, originalLength - 1);
                double fraction = pos - leftIndex;

                result[i] = originalValues[leftIndex] * (1 - fraction) + originalValues[rightIndex] * fraction;
            }

            return result;
        }

        private void AppendLabeledSequenceToJson(List<GyroscopeDataDto> normalizedBatch, bool isCorrect)
        {
            var filePath = "json_labeled_sequences.json";

            // 1. Felépítjük az aktuális ismétlés JSON tömbjét:
            //    [ {GyrosX..AccelZ}, ..., {IsCorrect: true/false} ]

            var currentSequence = new List<object>();

            // 13 db szenzor sor (normált adatokkal)
            for (int i = 0; i < normalizedBatch.Count; i++)
            {
                var d = normalizedBatch[i];

                currentSequence.Add(new
                {
                    GyrosX = d.GyrosX,
                    GyrosY = d.GyrosY,
                    GyrosZ = d.GyrosZ,
                    AccelX = d.AccelX,
                    AccelY = d.AccelY,
                    AccelZ = d.AccelZ
                });
            }

            // 14. elem: az IsCorrect jelölés
            currentSequence.Add(new
            {
                IsCorrect = isCorrect   // true vagy false az ML eredménye alapján
            });

            // 2. Betöltjük a teljes eddigi listát (ha létezik a fájl)

            List<List<object>> allSequences = new();

            if (File.Exists(filePath))
            {
                var existingJson = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    allSequences = JsonSerializer.Deserialize<List<List<object>>>(existingJson)
                                   ?? new List<List<object>>();
                }
            }

            // 3. Hozzáadjuk az aktuális ismétlést

            allSequences.Add(currentSequence);

            // 4. Szépen formázva visszaírjuk a fájlba

            var newJson = JsonSerializer.Serialize(allSequences, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, newJson);
        }

    }
}

