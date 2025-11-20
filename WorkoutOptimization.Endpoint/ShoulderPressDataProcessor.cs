using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Text.Json;
using WorkoutOptimization.Endpoint.Helpers;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Endpoint
{
    public class ShoulderPressDataProcessor : BackgroundService
    {
        private readonly ShoulderQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ExerciseHub> _hubContext;
        private readonly IShoulderDetector _repDetector;

        private int? _lastTrainingId = null;

        // vállból nyomás specifikus paraméterek
        public ShoulderPressDataProcessor(
            ShoulderQueue queue,
            IServiceScopeFactory scopeFactory,
            IHubContext<ExerciseHub> hubContext,
            IShoulderDetector repDetector)
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
                    // training váltás log (ezt meghagyhatod)
                    if (data.TrainingId != null && data.TrainingId != _lastTrainingId)
                    {
                        _lastTrainingId = (int)data.TrainingId;
                        using var scope = _scopeFactory.CreateScope();
                        var trainingRepo = scope.ServiceProvider.GetRequiredService<IRepository<Training>>();
                        var training = trainingRepo.Read(_lastTrainingId.Value);
                        Console.WriteLine($"[i] Új training vállból nyomás detektorral: Id={_lastTrainingId}");
                    }

                    // 👉 új: átadjuk a mintát a detektornak
                    var reps = _repDetector.AddSample(data);

                    // minden lezárt ismétlésre lefuttatjuk a batch feldolgozást
                    foreach (var rep in reps)
                    {
                        // rep: IReadOnlyList<GyroscopeDataDto>, de a metódus List-et kér → ToList()
                        await ProcessBatchAsync(rep.ToList());
                    }
                }

                await Task.Delay(10, stoppingToken);
            }
        }

        // ------------------------ KÖZÖS RÉSZ ------------------------

        private async Task ProcessBatchAsync(List<GyroscopeDataDto> batch)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var preprocessor= scope.ServiceProvider.GetRequiredService<IRepPreProcessor>();
                var shoulderLogic = scope.ServiceProvider.GetRequiredService<IShoulderPressLogic>();
                var exerciseLogic = scope.ServiceProvider.GetRequiredService<IExerciseResultLogic>();

                var normalizedBatch = preprocessor.NormalizeRep(batch);
                var converted= preprocessor.ToOnnxInput(normalizedBatch);

                var res = shoulderLogic.DataValidation(converted);
                AppendLabeledSequenceToJson(normalizedBatch.ToList(), res);

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


        private void AppendLabeledSequenceToJson(List<GyroscopeDataDto> normalizedBatch, bool isCorrect)
        {
            var filePath = "json_labeled_sequences.json";

            var currentSequence = new List<object>();

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

            currentSequence.Add(new
            {
                IsCorrect = isCorrect
            });

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

            allSequences.Add(currentSequence);

            var newJson = JsonSerializer.Serialize(allSequences, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, newJson);
        }
    }
}
