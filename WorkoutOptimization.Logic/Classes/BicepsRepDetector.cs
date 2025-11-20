using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Classes
{
    public class BicepsRepDetector : IBicepsRepDetector
    {
        private Axis _axis;
        private Func<GyroscopeDataDto, double> _getAxis;

        private readonly int _windowSize = 7;
        private readonly TimeSpan _minRepInterval = TimeSpan.FromMilliseconds(600);

        private DateTime? _lastRepTime = null;

        private List<GyroscopeDataDto> _repBuffer = new();
        private readonly Queue<double> _slidingWindow = new();
        private readonly Queue<GyroscopeDataDto> _rawWindow = new();

        public BicepsRepDetector(Axis axis = Axis.GyrosZ)
        {
            SetAxis(axis);
        }

        public void SetAxis(Axis axis)
        {
            _axis = axis;
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

            Console.WriteLine($"[i] Biceps rep detektálás tengelye: {_axis}");
        }

        public IEnumerable<IReadOnlyList<GyroscopeDataDto>> AddSample(GyroscopeDataDto data)
        {
            var completedReps = new List<IReadOnlyList<GyroscopeDataDto>>();

            // Bufferelés
            _repBuffer.Add(data);
            _rawWindow.Enqueue(data);
            _slidingWindow.Enqueue(_getAxis(data));

            // Csak akkor vizsgáljuk, ha megvan a teljes ablak
            if (_slidingWindow.Count == _windowSize)
            {
                var values = _slidingWindow.ToArray();
                int middleIndex = _windowSize / 2;
                double middle = values[middleIndex];

                // Lokális minimum detektálás (szigorú minimum a window-ban)
                bool isStrictMinimum = true;
                for (int i = 0; i < _windowSize; i++)
                {
                    if (i == middleIndex) continue;
                    if (middle >= values[i]) // ha van nála kisebb vagy egyenlő, akkor nem szigorú minimum
                    {
                        isStrictMinimum = false;
                        break;
                    }
                }

                // Forma ellenőrzése: völgy-szerű legyen (eleje és vége magasabb, mint a középső)
                bool hasValleyShape = values[0] > middle && values[_windowSize - 1] > middle;

                bool isPeakCandidate = isStrictMinimum && hasValleyShape;

                if (isPeakCandidate)
                {
                    // Időalapú szűrés: két ismétlés között legyen minimális idő
                    var now = DateTime.UtcNow;
                    if (_lastRepTime.HasValue && now - _lastRepTime.Value < _minRepInterval)
                    {
                        // Túl közel van az előző ismétléshez -> valószínűleg ugyanannak a mozdulatnak a "második völgye"
                        // -> ignoráljuk, és simán csúsztatjuk az ablakot
                        _slidingWindow.Dequeue();
                        _rawWindow.Dequeue();
                        return completedReps;
                    }

                    _lastRepTime = now;

                    Console.WriteLine($"[✓] Biceps ismétlés detektálva ({_axis} peak: {middle:F2}, idő: {now:HH:mm:ss.fff})");

                    // A repBuffer tartalmazza az eddigi adatokat,
                    // de ne adjuk át a peak utáni 3-at (hogy a következő ismétlés eleje bent maradjon)
                    int cutIndex = _repBuffer.Count - (_windowSize - 3);
                    if (cutIndex < 0) cutIndex = 0;

                    var repetition = _repBuffer.Take(cutIndex).ToList(); // csak a peak-ig
                    if (repetition.Count > 0)
                    {
                        completedReps.Add(repetition);
                    }

                    // Maradék visszatöltése (peak utáni pár minta marad bufferben)
                    _repBuffer = _repBuffer.Skip(cutIndex).ToList();

                    // Sliding window újratöltése a megmaradt elemek tengely-értékeivel
                    var remainingAxisVals = _repBuffer.Select(x => _getAxis(x)).ToList();
                    _slidingWindow.Clear();
                    foreach (var val in remainingAxisVals)
                        _slidingWindow.Enqueue(val);

                    _rawWindow.Clear();
                    foreach (var item in _repBuffer)
                        _rawWindow.Enqueue(item);
                }
                else
                {
                    // nincs peak -> csúsztatjuk az ablakot
                    _slidingWindow.Dequeue();
                    _rawWindow.Dequeue();
                }
            }

            return completedReps;
        }
    }
}
