using System;

using BrutalEvent.Models;
using BrutalEvent.Services.Abstract;

namespace BrutalEvent.Services.Events
{
    internal class ChaosEvent : LevelEvent
    {
        public override string GetEventName()
        {
            return $"<color=red>LEAVE THIS MOON! CHAOS!</color>";
        }

        public override LevelEvent CreateEvent() => new ChaosEvent();

        public override void OnLoadNewLevel(ref SelectableLevel newLevel, ConfigValues configs, float currentRate)
        {
            _enviroment.SetupEnemyChance(newLevel, currentRate, configs.Multiplier.Value);
            _enviroment.GenerateEnemiesEvent<FlowermanAI>(newLevel, currentRate);
            _enviroment.GenerateEnemiesEvent<BlobAI>(newLevel, currentRate);
            _enviroment.GenerateEnemiesEvent<SandSpiderAI>(newLevel, currentRate);
            _enviroment.GenerateEnemiesEvent<CentipedeAI>(newLevel, currentRate);
            _enviroment.GenerateEnemiesEvent<CrawlerAI>(newLevel, currentRate);
            _enviroment.GenerateEnemiesEvent<HoarderBugAI>(newLevel, currentRate, 2.0f);
            _enviroment.GenerateEnemiesEvent<SpringManAI>(newLevel, currentRate);
        }
    }
}
