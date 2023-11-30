using System;
using BrutalEvent.Enums;
using UnityEngine;

namespace BrutalEvent.Services.Abstract
{
    public class SpawnCurveGenerator
    {
        // Пример метода для создания кривой спавна
        public AnimationCurve CreateEnemySpawnCurve(float[] times, float[] values)
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

        public static SelectableLevel SetupEnemyChance(SelectableLevel newLevel, float currentEventRate)
        {
            newLevel.enemySpawnChanceThroughoutDay = new AnimationCurve(new[]
            {
                new Keyframe(0f, 0.1f + currentEventRate),
                new Keyframe(1f, 33f + currentEventRate)
            });

            newLevel.outsideEnemySpawnChanceThroughDay = new AnimationCurve(new[]
            {
                new Keyframe(0f, -10f + currentEventRate),
                new Keyframe(1f, 10f + currentEventRate),
            });

            return newLevel;
        }
    }
}
