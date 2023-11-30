using System.Linq;
using UnityEngine;
using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;
using HarmonyLib;

namespace BrutalEvent.Services.Events
{
    internal class ReplaceItemsEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=orange>DID YOU BUY FLASHLIGHT? RIGHT?</color>";
        }

        public override LevelEvent CreateEvent() => new ReplaceItemsEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            var terminal = Object.FindObjectOfType<Terminal>();
            var countItems = terminal.buyableItemsList.Length;
            var items = terminal.buyableItemsList.ToArray();
            terminal.buyableItemsList = null;
            
            for (int i = countItems; i > 0; i--)
            {
                terminal.buyableItemsList.AddItem(items[i]);
            }
        }
    }
}
