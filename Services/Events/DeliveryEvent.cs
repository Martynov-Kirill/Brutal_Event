using UnityEngine;
using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;
using Random = UnityEngine.Random;

namespace BrutalEvent.Services.Events
{
    internal class DeliveryEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"<color=green>EZ DELIVERY!</color>";
        }

        public override LevelEvent CreateEvent() => new DeliveryEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, ConfigValues configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            var terminal = Object.FindObjectOfType<Terminal>();
            int countItems = Random.Range(3, 9);
            for (int i = 0; i < countItems; i++)
            {
                int item = Random.Range(0, 6);
                terminal.orderedItemsFromTerminal.Add(item);
            }
        }
    }
}
