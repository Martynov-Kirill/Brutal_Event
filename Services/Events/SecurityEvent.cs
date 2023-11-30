using System;

using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class SecurityEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=orange>COMPANY SECURITY SYSTEM! GLHF :)</color>";
        }

        public override LevelEvent CreateEvent() => new SecurityEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, ConfigValues configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            var spawnableMapObjects = newLevel.spawnableMapObjects;

            foreach (var mapObject in spawnableMapObjects)
            {
                if (mapObject.prefabToSpawn.GetComponentInChildren<Turret>() != null)
                {
                    mapObject.numberToSpawn = _curveGenerator.CreateSpawnCurve(
                        new[] { 0f, 25f + currentRate },
                        new[] { 100f, 100f + currentRate * 3f });
                }
            }
        }
    }
}
