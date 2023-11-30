using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;
using UnityEngine;

namespace BrutalEvent.Services.Events
{
    internal class GoToRentEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=green>GO TO RENT!</color>";
        }

        public override LevelEvent CreateEvent() => new GoToRentEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            var terminal = Object.FindObjectOfType<Terminal>();
            foreach (var item in terminal.buyableItemsList)
            {
                item.creditsWorth += (int)(currentRate + item.creditsWorth * 0.5);
            }
        }
    }
}
