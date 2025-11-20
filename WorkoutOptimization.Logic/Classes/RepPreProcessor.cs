using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Classes
{
    public class RepPreProcessor : IRepPreProcessor
    {
        private const int TargetLength = 13;
        public IReadOnlyList<GyroscopeDataDto> NormalizeRep(IReadOnlyList<GyroscopeDataDto> rep)
        {
            double[] gyrosX = rep.Select(d => (double)d.GyrosX).ToArray();
            double[] gyrosY = rep.Select(d => (double)d.GyrosY).ToArray();
            double[] gyrosZ = rep.Select(d => (double)d.GyrosZ).ToArray();
            double[] accelX = rep.Select(d => (double)d.AccelX).ToArray();
            double[] accelY = rep.Select(d => (double)d.AccelY).ToArray();
            double[] accelZ = rep.Select(d => (double)d.AccelZ).ToArray();

            double[] normGyrosX = NormalizeLength(gyrosX, TargetLength);
            double[] normGyrosY = NormalizeLength(gyrosY, TargetLength);
            double[] normGyrosZ = NormalizeLength(gyrosZ, TargetLength);
            double[] normAccelX = NormalizeLength(accelX, TargetLength);
            double[] normAccelY = NormalizeLength(accelY, TargetLength);
            double[] normAccelZ = NormalizeLength(accelZ, TargetLength);

            var normalized = new List<GyroscopeDataDto>(TargetLength);
            for (int i = 0; i < TargetLength; i++)
            {
                normalized.Add(new GyroscopeDataDto
                {
                    GyrosX = (float)normGyrosX[i],
                    GyrosY = (float)normGyrosY[i],
                    GyrosZ = (float)normGyrosZ[i],
                    AccelX = (float)normAccelX[i],
                    AccelY = (float)normAccelY[i],
                    AccelZ = (float)normAccelZ[i],
                });
            }

            return normalized;
        }

        public float[,,] ToOnnxInput(IReadOnlyList<GyroscopeDataDto> rep)
        {
            var normalized = NormalizeRep(rep);
            var converted = new float[1, TargetLength, 6];

            for (int i = 0; i < normalized.Count; i++)
            {
                var d = normalized[i];
                converted[0, i, 0] = d.GyrosX;
                converted[0, i, 1] = d.GyrosY;
                converted[0, i, 2] = d.GyrosZ;
                converted[0, i, 3] = d.AccelX;
                converted[0, i, 4] = d.AccelY;
                converted[0, i, 5] = d.AccelZ;
            }

            return converted;
        }

        private static double[] NormalizeLength(double[] originalValues, int targetLength)
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

                result[i] = originalValues[leftIndex] * (1 - fraction)
                          + originalValues[rightIndex] * fraction;
            }

            return result;
        }
    }
}
