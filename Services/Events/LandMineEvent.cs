using System;
using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;
using UnityEngine;

namespace BrutalEvent.Services.Events
{
    internal class LandMineEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"MINE MINE MINE!";
        }

        public override LevelEvent CreateEvent()
        {
            return new LandMineEvent();
        }

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            SpawnableMapObject[] spawnableMapObjects = newLevel.spawnableMapObjects;

            foreach (var mapObject in spawnableMapObjects)
            {
                if (mapObject.prefabToSpawn.GetComponentInChildren<Landmine>() != null)
                {
                    mapObject.numberToSpawn = _curveGenerator.CreateEnemySpawnCurve(
                        new[] { 0f, 10f * 3f },
                        new[] { 0.4f, 50f + currentRate * 3f });
                }
            }
        }
    }
}