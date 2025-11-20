using Microsoft.AspNetCore.SignalR;
using ScottPlot;
using System.Collections.Concurrent;
using System.Text.Json;
using WorkoutOptimization.Endpoint.Helpers;
using WorkoutOptimization.Logic.Classes;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;
using WorkoutOptimization.Repository;
using WorkoutOptimization.Repository.Migrations;

namespace WorkoutOptimization.Endpoint
{
    public class BicepsCurlDataProcessor : BackgroundService
    {
        private readonly BicepsQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ExerciseHub> _hubContext;
        private readonly IBicepsRepDetector _repDetector;
        private int? _lastTrainingId = null;


        public BicepsCurlDataProcessor(
            BicepsQueue queue,
            IServiceScopeFactory scopeFactory,
            IHubContext<ExerciseHub> hubContext,
            IBicepsRepDetector repDetector
        )
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _repDetector = repDetector;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                while (_queue.TryDequeue(out var data))
                {
                    // Training váltás kezelése (ha később vissza akarod hozni az axis-t DB-ből)
                    if (data.TrainingId != null)
                    {
                        int currentId = (int)data.TrainingId;
                        if (currentId != _lastTrainingId)
                        {
                            _lastTrainingId = currentId;
                            using var scope = _scopeFactory.CreateScope();
                            var trainingRepo = scope.ServiceProvider.GetRequiredService<IRepository<Training>>();
                            var training = trainingRepo.Read((int)data.TrainingId);
                            // Ha a Training-ben tárolod, vissza tudod hozni:
                            // pl. (ha akarod később) ((BicepsRepDetector)_repDetector).SetAxis(training.Axis);
                        }
                    }

                    // 👉 Itt hívjuk meg a detektort, ami már elvégzi a sliding-window logikát
                    var reps = _repDetector.AddSample(data);

                    // Minden lezárt ismétlésre lefuttatjuk a batch feldolgozást
                    foreach (var rep in reps)
                    {
                        await ProcessBatchAsync(rep.ToList());
                    }
                }

                await Task.Delay(10, stoppingToken);
            }
        }

        private async Task ProcessBatchAsync(List<GyroscopeDataDto> batch)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                // 1) Szolgáltatások
                var preprocessor = scope.ServiceProvider.GetRequiredService<IRepPreProcessor>();
                var bicepsLogic = scope.ServiceProvider.GetRequiredService<IBicepsCurlLogic>();
                var exerciseLogic = scope.ServiceProvider.GetRequiredService<IExerciseResultLogic>();

                // 2) Normalizálás + ONNX input
                var normalizedBatch = preprocessor.NormalizeRep(batch);          // fix hossz, pl. 13
                var converted = preprocessor.ToOnnxInput(normalizedBatch);       // [1, T, 6]

                // 3) Grafikonhoz szükséges adatok (eredeti + normalizált)

                // Eredeti (nyers) tengelyek
                double[] gyrosX = batch.Select(d => (double)d.GyrosX).ToArray();
                double[] gyrosY = batch.Select(d => (double)d.GyrosY).ToArray();
                double[] gyrosZ = batch.Select(d => (double)d.GyrosZ).ToArray();
                double[] accelX = batch.Select(d => (double)d.AccelX).ToArray();
                double[] accelY = batch.Select(d => (double)d.AccelY).ToArray();
                double[] accelZ = batch.Select(d => (double)d.AccelZ).ToArray();

                // Normalizált tengelyek (már a normalizedBatch-ből)
                double[] normGyrosX = normalizedBatch.Select(d => (double)d.GyrosX).ToArray();
                double[] normGyrosY = normalizedBatch.Select(d => (double)d.GyrosY).ToArray();
                double[] normGyrosZ = normalizedBatch.Select(d => (double)d.GyrosZ).ToArray();
                double[] normAccelX = normalizedBatch.Select(d => (double)d.AccelX).ToArray();
                double[] normAccelY = normalizedBatch.Select(d => (double)d.AccelY).ToArray();
                double[] normAccelZ = normalizedBatch.Select(d => (double)d.AccelZ).ToArray();

                // Eredeti indexek (0 ... batch.Count-1)
                double[] origIndex = Enumerable.Range(0, batch.Count)
                                               .Select(i => (double)i)
                                               .ToArray();

                // Normalizált indexek (0 ... normalizedBatch.Count-1)
                double[] normIndex = Enumerable.Range(0, normalizedBatch.Count)
                                               .Select(i => (double)i)
                                               .ToArray();

                // 4) ScottPlot grafikon (ugyanaz a vizuál, csak más forrásból jönnek a norm adatok)
                var plt = new ScottPlot.Plot(800, 600);

                // Eredeti adatok scatter
                plt.AddScatter(origIndex, gyrosX, label: "Gyro X (orig)", color: System.Drawing.Color.Red, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
                plt.AddScatter(origIndex, gyrosY, label: "Gyro Y (orig)", color: System.Drawing.Color.Green, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
                plt.AddScatter(origIndex, gyrosZ, label: "Gyro Z (orig)", color: System.Drawing.Color.Blue, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);

                plt.AddScatter(origIndex, accelX, label: "Accel X (orig)", color: System.Drawing.Color.Orange, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
                plt.AddScatter(origIndex, accelY, label: "Accel Y (orig)", color: System.Drawing.Color.Purple, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);
                plt.AddScatter(origIndex, accelZ, label: "Accel Z (orig)", color: System.Drawing.Color.Brown, markerSize: 5, markerShape: ScottPlot.MarkerShape.filledCircle);

                // Normalizált adatok scatter
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

                // 5) ML validáció + címkézett json + SignalR + DB

                var res = bicepsLogic.DataValidation(converted);

                // labeled sequences json (normalizált adatokkal)
                AppendLabeledSequenceToJson(normalizedBatch.ToList(), res);

                string message = res ? "Helyes" : "Helytelen";
                var trainingId = batch.First().TrainingId;

                var exerciseResult = new ExerciseResultCreateModel
                {
                    IsCorrect = res,
                    TrainingId = trainingId ?? 0
                };

                await _hubContext.Clients.All.SendAsync("ReceivePrediction", message);
                await exerciseLogic.CreateExerciseResult(exerciseResult);
            }
        }


        private void AppendLabeledSequenceToJson(List<GyroscopeDataDto> normalizedBatch, bool isCorrect)
        {
            var filePath = "json_labeled_sequences.json";

            // Egy ismétlés JSON tömbje:
            // [ {GyrosX..AccelZ}, ..., {IsCorrect: true/false} ]

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
                IsCorrect = isCorrect
            });

            // Teljes eddigi lista betöltése
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

            // Aktuális ismétlés hozzáadása
            allSequences.Add(currentSequence);

            // Visszaírás
            var newJson = JsonSerializer.Serialize(allSequences, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, newJson);
        }
    }
}
