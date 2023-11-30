using System;
using UnityEngine;

namespace BrutalEvent.Services.Abstract
{
    public class SpawnCurveGenerator
    {
        // Пример метода для создания кривой спавна
        public AnimationCurve CreateSpawnCurve(float[] times, float[] values)
        {
            if (times.Length != values.Length)
            {
                throw new ArgumentException("Times and values arrays must have the same length.");
            }

            Keyframe[] keyframes = new Keyframe[times.Length];
            for (int i = 0; i < times.Length; i++)
            {
                keyframes[i] = new Keyframe(times[i], values[i]);
            }

            return new AnimationCurve(keyframes);
        }
    }
}
