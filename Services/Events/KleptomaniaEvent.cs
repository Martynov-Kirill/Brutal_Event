using System;
using System.Runtime.InteropServices;
using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class KleptomaniaEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"<color=orange>KLEPTOMANIA!</color>";
        }

        public override LevelEvent CreateEvent()
        {
            return new KleptomaniaEvent();
        }

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, ConfigValues configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<HoarderBugAI>(newLevel,currentRate, 2.5f);
        }
    }
}
