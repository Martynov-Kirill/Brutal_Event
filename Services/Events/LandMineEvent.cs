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
            return $"<color=red>MINE MINE MINE!</color>";
        }

        public override LevelEvent CreateEvent()
        {
            return new LandMineEvent();
        }

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            foreach (var mapObject in newLevel.spawnableMapObjects)
            {
                if (mapObject.prefabToSpawn.GetComponentInChildren<Landmine>() != null)
                {
                    mapObject.numberToSpawn = _curveGenerator.CreateSpawnCurve(
                        new[] { 0f, 25f * configs.Multiplier.Value},
                        new[] { 100f, 500f + currentRate * 3f });
                }
            }
        }
    }
}