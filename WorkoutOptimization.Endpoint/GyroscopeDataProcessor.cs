
using System.Collections.Concurrent;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;
using WorkoutOptimization.Repository;
using ScottPlot;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using WorkoutOptimization.Endpoint.Helpers;
using WorkoutOptimization.Repository.Migrations;

namespace WorkoutOptimization.Endpoint
{
    public class GyroscopeDataProcessor : BackgroundService
    {
        private readonly ConcurrentQueue<GyroscopeDataDto> _queue;
        IBicepsCurlLogic _bicepsCurlLogic;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ExerciseHub> _hubContext;

        public GyroscopeDataProcessor(ConcurrentQueue<GyroscopeDataDto> queue,
            IServiceScopeFactory scopeFactory, IBicepsCurlLogic bicepsCurlLogic, IHubContext<ExerciseHub> hubContext)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _bicepsCurlLogic= bicepsCurlLogic;
            _hubContext=hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<GyroscopeDataDto> repBuffer = new();
            Queue<double> slidingWindow = new(); // csak a GyrosZ értékek
            Queue<GyroscopeDataDto> rawWindow = new(); // a teljes objektumok, feldolgozáshoz

            const int windowSize = 7;

            while (!stoppingToken.IsCancellationRequested)
            {
                while (_queue.TryDequeue(out var data))
                {
                    repBuffer.Add(data);
                    rawWindow.Enqueue(data);
                    slidingWindow.Enqueue(data.GyrosZ);

                    // Csak akkor vizsgáljuk, ha megvan a teljes ablak
                    if (slidingWindow.Count == windowSize)
                    {
                        var values = slidingWindow.ToArray();

                        int middleIndex = windowSize / 2;
                        double middle = values[middleIndex];

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
                            Console.WriteLine($"[✓] Ismétlés detektálva (GyrosZ peak: {middle:F2})");

                            // A repBuffer tartalmazza az eddigi adatokat, de ne adjuk át a peak utáni 3-at
                            int cutIndex = repBuffer.Count - (windowSize - 3);
                            var repetition = repBuffer.Take(cutIndex).ToList(); // Csak a peak-ig
                            await ProcessBatchAsync(repetition);

                            // A repBuffer-be csak a peak utáni adatokat tesszük vissza
                            repBuffer = repBuffer.Skip(cutIndex).ToList();

                            // Sliding window újratöltése az utolsó 3 értékkel
                            var lastZs = repBuffer.Select(x => x.GyrosZ).ToList();
                            slidingWindow.Clear();
                            foreach (var val in lastZs)
                                slidingWindow.Enqueue(val);

                            rawWindow.Clear();
                            foreach (var item in repBuffer)
                                rawWindow.Enqueue(item);
                        }
                        else
                        {
                            // Csúsztatjuk az ablakot
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
            int targetLength = 13; // fix hosszúság

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

            var json = JsonSerializer.Serialize(normalizedBatch);
            File.WriteAllText("json_normalized.json", json);

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
            var res = _bicepsCurlLogic.DataValidation(converted);
            string message = res ? "Helyes gyakorlat!" : "Hibás végrehajtás!";
            await _hubContext.Clients.All.SendAsync("ReceivePrediction", message);

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
    }
}
