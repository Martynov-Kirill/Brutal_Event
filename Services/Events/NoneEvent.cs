using System;

using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    public class NoneEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return "<color=green>NONE EVENT</color>";
        }

        public override LevelEvent CreateEvent() => new NoneEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, ConfigValues configs, float currentRate)
        {
            _enviroment.NormalizeEnemiesRarity(newLevel);
        }
    }
}
