using System;

using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class TremorsEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=red>DID YOU SAW THIS MOVIE?)</color>";
        }

        public override LevelEvent CreateEvent() => new TremorsEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            var spawnableMapObjects = newLevel.spawnableMapObjects;

            for (int i = 0; i < newLevel.outsideEnemySpawnChanceThroughDay.length; i++)
            {
                if (newLevel.OutsideEnemies[i].enemyType.enemyName != nameof(SandWormAI))
                    newLevel.OutsideEnemies[i].rarity = 0;
            }

            foreach (var mapObject in spawnableMapObjects)
            {
                if (mapObject.prefabToSpawn.GetComponentInChildren<SandWormAI>() != null)
                {
                    mapObject.numberToSpawn = _curveGenerator.CreateSpawnCurve(
                        new[] { 0f, 15f },
                        new[] { 0.5f, 50f + currentRate * 3f });
                }
            }
        }

    }
}
