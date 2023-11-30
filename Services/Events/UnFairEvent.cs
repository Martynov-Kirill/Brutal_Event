using System;
using System.Runtime.InteropServices;
using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class UnFairEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=green>THIS IS UNFAIR! :(</color>";
        }

        public override LevelEvent CreateEvent() => new UnFairEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, Config configs, float currentRate)
        {
            _enviroment.SetupLevelScrap(newLevel, configs, 2.5f);
            _enviroment.ResetEnemiesRarity(newLevel);
        }
    }
}
