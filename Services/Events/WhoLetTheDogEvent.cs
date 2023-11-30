using System;
using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class WhoLetTheDogEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=red>WHO LET THE DOGS OUT, WOOF WOOF</color>";
        }

        public override LevelEvent CreateEvent() => new WhoLetTheDogEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            var spawnableMapObjects = newLevel.spawnableMapObjects;

            for (int i = 0; i < newLevel.outsideEnemySpawnChanceThroughDay.length; i++)
            {
                if (newLevel.OutsideEnemies[i].enemyType.enemyName != nameof(MouthDogAI))
                    newLevel.OutsideEnemies[i].rarity = 0;
            }

            foreach (var mapObject in spawnableMapObjects)
            {
                if (mapObject.prefabToSpawn.GetComponentInChildren<MouthDogAI>() != null)
                {
                    mapObject.numberToSpawn = _curveGenerator.CreateSpawnCurve(
                        new[] { 0f, 50f },
                        new[] { 100f, 100f + currentRate * 3f });
                }
            }
        }
    }
}
