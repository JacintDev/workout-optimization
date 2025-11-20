using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutOptimization.Models.Models;
using WorkoutOptimization.Logic.Interfaces;

public class ShoulderPressRepDetector : IShoulderDetector
{
    private const int SmoothWindow = 7;
    private readonly TimeSpan MinRepInterval = TimeSpan.FromMilliseconds(800);

    private readonly Queue<double> _smoothY = new();
    private bool _inRep = false;

    private double _lastY = 0;
    private bool _wentUp = false;

    private readonly List<GyroscopeDataDto> _repBuffer = new();

    private DateTime? _lastRepTime = null;

    public IEnumerable<IReadOnlyList<GyroscopeDataDto>> AddSample(GyroscopeDataDto data)
    {
        var output = new List<IReadOnlyList<GyroscopeDataDto>>();

        double rawY = data.AccelY;

        // Simítás
        _smoothY.Enqueue(rawY);
        if (_smoothY.Count > SmoothWindow)
            _smoothY.Dequeue();

        double y = _smoothY.Average();

        var now = DateTime.UtcNow;

        if (!_inRep)
        {
            // Rep indul amikor lefelé megy (csökken az accelY)
            if (y < _lastY)
            {
                _inRep = true;
                _wentUp = false;
                _repBuffer.Clear();
            }
        }

        if (_inRep)
        {
            _repBuffer.Add(data);

            // Megjött a peak → onnantól kezdjük keresni a völgyet
            if (!_wentUp && y > _lastY)
            {
                _wentUp = true;
            }

            // Völgy megtalálva → ismétlés vége
            if (_wentUp && y < _lastY)
            {
                // Minimum időablak → csak akkor fogadjuk el, ha letelt
                bool repAllowed =
                    !_lastRepTime.HasValue ||
                    now - _lastRepTime.Value >= MinRepInterval;

                if (repAllowed)
                {
                    var finishedRep = _repBuffer.ToList();
                    output.Add(finishedRep);
                    _lastRepTime = now;
                }

                // Állapot reset
                _inRep = false;
                _wentUp = false;
                _repBuffer.Clear();
            }
        }

        _lastY = y;
        return output;
    }
}
